using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Dto;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;
using CommonDLL.Static;
using Unity.Burst.Intrinsics;
using UnityEngine;

using Pvp = BlackTemple.EpicMine.Core.Pvp;
using Random = UnityEngine.Random;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine.UIElements;


public class RatingsController
{
    private readonly int _updatePeriod = 60 * 60 * 12;

    public class Player
    {
        public string Id;
        public string Name;
        public string Location;
        public long CreationDate;
        public string PickAxe;
    }

    public class RatingInfo
    {
        public long NextUpdate;
        public List<PlayerMineRating> NewBieTop;
        public List<PlayerMineRating> MineTop;
        public List<PlayerPvpRating> PvpTop;

        public List<Player> Players;


        public RatingInfo()
        {
            MineTop = new List<PlayerMineRating>();
            PvpTop = new List<PlayerPvpRating>();
            NewBieTop = new List<PlayerMineRating>();
            Players = new List<Player>();
        }
    }

    private RatingInfo _data;

    private const string FileName = "ratings";

    private readonly IStorageService _storageService = new JsonDiskStorageService();

    private PlayerMineRating _playerMineRating;

    public void CalculateMineRating()
    {

        var playerData = App.Instance.Player;

        if (string.IsNullOrEmpty(playerData.Nickname))
            return;

        var tierNumber = playerData.Dungeon.Tiers.Count;
        var mineNumber = 0;
        var rating = 0;
        var hardcoreRating = 0;
        var prestige = playerData.Prestige;

        if (tierNumber > 0)
            tierNumber -= 1;

        foreach (var mine in playerData.Dungeon.Tiers[tierNumber].Mines)
        {
            if (mine.IsComplete)
                mineNumber = mine.Number;
        }

        for (var index = 0; index < playerData.Dungeon.Tiers.Count; index++)
        {
            var tier = playerData.Dungeon.Tiers[index];
            if (tier.IsOpen)
            {
                tierNumber = index;
            }

            foreach (var mine in tier.Mines)
            {
                if (mine.IsComplete)
                {
                    rating += mine.Rating;
                    hardcoreRating += mine.HardcoreRating;
                }
            }
        }

        _playerMineRating =
            new PlayerMineRating
            {
                UserNick = playerData.Nickname,
                UserId = playerData.Id,
                Tier = tierNumber + 1,
                Mine = mineNumber + 1,
                Prestige = prestige,
                Rating = rating,
                HardCoreRating = hardcoreRating,
            };

    }


    public RatingsController()
    {
        Initialize();
    }


    public void Save()
    {
        if (App.Instance == null)
            return;

        _storageService.Save(FileName, _data);
    }

    public void Clear()
    {
        _storageService.Remove(FileName);
    }

    public void Initialize()
    {
        GetData();

        RemoveOldUsers();

        LoadBots();

        UpdateSelf();

        Calculate();

        Save();
    }

    public void Refresh()
    {
        UpdateSelf();
        Calculate();
    }

    public void UpdateSelf()
    {
        if (string.IsNullOrEmpty(App.Instance.Player.Nickname))
            return;

        var player = GetPlayer(App.Instance.Player.Id);

        if (player == null)
        {
            player = new Player()
            {
                Id = App.Instance.Player.Id,
                Name = App.Instance.Player.Nickname,
                Location = App.Instance.Player.Location,
                CreationDate = App.Instance.Player.CreationDate,
            };
        }


        _playerMineRating = _data.MineTop.Find(x => x.UserId == player.Id);

        if (_playerMineRating == null)
        {
            CalculateMineRating();

            if (_playerMineRating != null)
                _data.MineTop.Add(_playerMineRating);
        }

        player.Name = App.Instance.Player.Nickname;


        var pvp = _data.PvpTop.Find(x => x.UserId == player.Id);
        if (pvp == null)
        {
            _data.PvpTop.Add(new PlayerPvpRating()
            {
                League = App.Instance.Player.Pvp.CurrentLeagueLvl,
                Rating = App.Instance.Player.Pvp.Rating,
                UserId = App.Instance.Player.Id,
                UserNick = App.Instance.Player.Nickname
            });
        }

        var newbie = _data.NewBieTop.Find(x => x.UserId == player.Id);
        if (newbie == null)
        {
            _data.NewBieTop.Add(_playerMineRating);
        }
    }

    public void GetData()
    {
        _data = _storageService.Load<RatingInfo>(FileName);

        if (_data == null)
        {
            _data = new RatingInfo();
            return;
        }

        if (_data.MineTop == null)
            _data.MineTop = new List<PlayerMineRating>();

        if (_data.NewBieTop == null)
            _data.NewBieTop = new List<PlayerMineRating>();

        if (_data.PvpTop == null)
            _data.PvpTop = new List<PlayerPvpRating>();

        if (_data.Players == null)
            _data.Players = new List<Player>();


        for (var index = 0; index < _data.PvpTop.Count; index++)
        {
            if (_data.PvpTop[index] == null)
            {
                _data.PvpTop.Remove(_data.PvpTop[index]);
            }
        }
        for (var index = 0; index < _data.MineTop.Count; index++)
        {
            if (_data.MineTop[index] == null)
            {
                _data.MineTop.Remove(_data.MineTop[index]);
                index--;

            }
        }
        for (var index = 0; index < _data.NewBieTop.Count; index++)
        {
            if (_data.NewBieTop[index] == null)
            {
                _data.NewBieTop.Remove(_data.NewBieTop[index]);
                index--;
            }
        }
        for (var index = 0; index < _data.Players.Count; index++)
        {
            if (_data.Players[index] == null)
            {
                _data.Players.Remove(_data.Players[index]);
                index--;
            }
        }
    }

    public void SetBotsForNewbieRating()
    {

    }

    public CommonDLL.Static.Pickaxe GetBotRandomPickAxe(Player bot)
    {
        var mineRating = _data.MineTop.Find(x => x.UserId == bot.Id && x.UserNick == bot.Name);


        // Debug.Log(bot.Id +":" + bot.Name);
        var all = App.Instance.StaticData.Pickaxes.FindAll(x => x.Type != PickaxeType.Mythical &&
                                                                (x.RequiredTierNumber <= mineRating.Tier +
                                                                 UnityEngine.Random.Range(-5, 0) &&
                                                                 x.RequiredTierNumber >= mineRating.Tier +
                                                                 UnityEngine.Random.Range(0, 5)));

        if (all.Count <= 0)
        {
            all = App.Instance.StaticData.Pickaxes.FindAll(x =>
                x.Type != PickaxeType.Mythical && x.Type != PickaxeType.Donate && x.Type != PickaxeType.God);

            return all.RandomElement();
        }

        return all.RandomElement();
    }

    public void RemoveOldUsers()
    {
        for (var index = 0; index < _data.NewBieTop.Count; index++)
        {
            var playerMineRating = _data.NewBieTop[index];
            if (playerMineRating.UserId != App.Instance.Player.Id)
            {
                var existPlayer = _data.Players.Find(x =>
                    x.Id == playerMineRating.UserId && x.Name == playerMineRating.UserNick);

                if (existPlayer == null)
                {
                    _data.NewBieTop.Remove(_data.NewBieTop[index]);

                    index--;
                }

                var samePlayer = _data.NewBieTop.Find(x =>
                    x.UserId == playerMineRating.UserId && x.UserNick == playerMineRating.UserNick &&
                    x != playerMineRating);

                if (samePlayer != null)
                {
                    _data.NewBieTop.Remove(samePlayer);
                }
            }
        }
        for (var index = 0; index < _data.PvpTop.Count; index++)
        {
            var playerMineRating = _data.PvpTop[index];
            if (playerMineRating.UserId != App.Instance.Player.Id)
            {
                var existPlayer = _data.Players.Find(x =>
                    x.Id == playerMineRating.UserId && x.Name == playerMineRating.UserNick);

                if (existPlayer == null)
                {
                    _data.PvpTop.Remove(_data.PvpTop[index]);
                    index--;
                }


                var samePlayer = _data.PvpTop.Find(x =>
                    x.UserId == playerMineRating.UserId && x.UserNick == playerMineRating.UserNick &&
                    x != playerMineRating);

                if (samePlayer != null)
                {
                    _data.PvpTop.Remove(samePlayer);
                }
            }
        }

        for (var index = 0; index < _data.Players.Count; index++)
        {
            var dataPlayer = _data.Players[index];
            if (dataPlayer.Id != App.Instance.Player.Id)
            {
                if (TimeManager.Instance.NowUnixSeconds > dataPlayer.CreationDate + 60 * 60 * 24 * 20)
                {
                    _data.Players.Remove(_data.Players[index]);

                    var pvp = _data.PvpTop.Find(x => x.UserId == dataPlayer.Id && x.UserNick == dataPlayer.Name);

                    if (pvp != null)
                    {
                        _data.PvpTop.Remove(pvp);
                    }

                    var newBie = _data.NewBieTop.Find(x => x.UserId == dataPlayer.Id && x.UserNick == dataPlayer.Name);

                    if (newBie != null)
                    {
                        _data.NewBieTop.Remove(newBie);
                    }

                    index--;
                }
            }
        }
    }

    public void LoadBots()
    {
        var staticData = App.Instance.StaticData;

        foreach (var staticDataPvpBot in staticData.PvpBots)
        {
            foreach (var botName in staticDataPvpBot.Names)
            {
                var name = botName;
                var bot = _data.Players.Find(x => x.Id == staticDataPvpBot.Id && x.Name == name);

                if (bot == null)
                {
                    var random = UnityEngine.Random.Range(staticDataPvpBot.MinRatingOffset,
                        staticDataPvpBot.MaxRatingOffset);

                    var player = new Player()
                    {
                        Id = staticDataPvpBot.Id,
                        Name = botName,
                        Location = Pvp.CountryCodes[
                            UnityEngine.Random.Range(0, Pvp.CountryCodes.Count)],
                        CreationDate = TimeManager.Instance.NowUnixSeconds - UnityEngine.Random.Range(0, 360 * 60 * 60 * 24)
                    };

                    _data.Players.Add(player);

                    _data.PvpTop.Add(new CommonDLL.Dto.PlayerPvpRating
                    {
                        Rating = random,
                        UserId = staticDataPvpBot.Id,
                        UserNick = name,
                        League = PvpHelper.GetLeagueByRating(staticDataPvpBot.MaxRatingOffset),
                    });

                    _data.MineTop.Add(new CommonDLL.Dto.PlayerMineRating()
                    {
                        Rating = random,
                        UserId = staticDataPvpBot.Id,
                        UserNick = name,
                        HardCoreRating = UnityEngine.Random.Range(0, 5000),
                        Mine = UnityEngine.Random.Range(1, 6),
                        OldPosition = 0,
                        Position = 0,
                        Prestige = UnityEngine.Random.Range(0, 6),
                        Tier = UnityEngine.Random.Range(1, 49)
                    });

                    player.PickAxe = GetBotRandomPickAxe(player).Id;
                }
            }

        }
    }

    public List<PlayerPvpRating> GetPvpRating()
    {
        if (_data.PvpTop.Count <= 0)
        {
            return new List<CommonDLL.Dto.PlayerPvpRating>();
        }
        else
        {
            var max = _data.PvpTop.Count <= 100 ? _data.PvpTop.Count : 100;

            var ratingList = _data.PvpTop.GetRange(0, max);
            var rating = new List<CommonDLL.Dto.PlayerPvpRating>();

            for (var index = 0; index < ratingList.Count; index++)
            {
                var playerMineRating = ratingList[index];
                rating.Add(playerMineRating);
            }

            var my = ratingList.Find(x => x.UserId == App.Instance.Player.Id);


            if (my == null)
            {
                var res = GetPlayerPvpRating(GetPlayer(App.Instance.Player.Id));

                if (res != null)
                {
                    rating.Add(res);
                }
            }
            return rating;
        }
    }


    public List<PlayerMineRating> GetNewbieMineRating()
    {
        if (_data.NewBieTop.Count <= 0)
            return new List<PlayerMineRating>();
        else
        {
            var max = _data.NewBieTop.Count <= 100 ? _data.NewBieTop.Count : 100;

            var ratingList = _data.NewBieTop.GetRange(0, max);
            var rating = new List<PlayerMineRating>();

            for (var index = 0; index < ratingList.Count; index++)
            {
                var playerMineRating = ratingList[index];
                rating.Add(playerMineRating);
            }


            var my = ratingList.Find(x => x.UserId == App.Instance.Player.Id);
            if (my == null)
            {
                var res = GetPlayerNewbieRating(GetPlayer(App.Instance.Player.Id));
                if (res != null)
                {
                    rating.Add(res);
                }
            }


            return rating;
        }
    }

    public List<PlayerMineRating> GetMineRating()
    {
        if (_data.MineTop.Count <= 0)
            return new List<PlayerMineRating>();
        else
        {
            var max = _data.MineTop.Count <= 100 ? _data.MineTop.Count : 100;

            var ratingList = _data.MineTop.GetRange(0, max);
            var rating = new List<PlayerMineRating>();

            for (var index = 0; index < ratingList.Count; index++)
            {
                var playerMineRating = ratingList[index];
                rating.Add(playerMineRating);
            }
            var my = ratingList.Find(x => x.UserId == App.Instance.Player.Id);
            if (my == null)
            {
                var res = GetPlayerMineRating(GetPlayer(App.Instance.Player.Id));
                if (res != null)
                {
                    rating.Add(res);
                }
            }

            return rating;
        }
    }


    private void CalculateAllNewbieRating()
    {
        var usersRatings = new List<PlayerMineRating>(_data.MineTop);

        var idList = new List<string>();
        foreach (var playerMineRating in usersRatings)
        {
            idList.Add(playerMineRating.UserId);
        }

        var users = _data.Players.FindAll(x => idList.Contains(x.Id)).ToList();

        var endDate = TimeManager.Instance.NowUnixSeconds - 30 * 24 * 60 * 60;

        for (var index = 0; index < usersRatings.Count; index++)
        {
            var playerMineRating = usersRatings[index];
            var user = users.Find(x => x.Id == playerMineRating.UserId);

            if (user == null || user.CreationDate < endDate)
            {
                usersRatings.Remove(playerMineRating);
                index--;
            }
        }

        usersRatings.Sort((a, b) =>
        {
            if (a.Prestige > b.Prestige)
                return -1;

            if (a.Prestige < b.Prestige)
                return 1;

            if (a.Tier > b.Tier)
                return -1;

            if (a.Tier < b.Tier)
                return 1;

            if (a.Mine > b.Mine)
                return -1;

            if (a.Mine < b.Mine)
                return 1;

            var aRating = 0;
            aRating += a.Rating;
            aRating += (a.HardCoreRating * 2);

            var bRating = 0;
            bRating += b.Rating;
            bRating += (b.HardCoreRating * 2);

            if (aRating > bRating)
                return -1;

            return aRating < bRating ? 1 : 0;
        });

        var ratingList = new List<PlayerMineRating>();
        foreach (var playerMineRating in usersRatings)
        {
            ratingList.Add(playerMineRating);
        }

        _data.NewBieTop = ratingList;

    }


    private void CalculateAllMinePvpRating()
    {

        var usersRatings = _data.PvpTop;

        usersRatings.Sort((a, b) =>
        {

            var aRating = 0;
            aRating += a.Rating;

            var bRating = 0;
            bRating += b.Rating;

            if (aRating > bRating)
                return -1;

            return aRating < bRating ? 1 : 0;

        });

        var ratingList = new List<PlayerPvpRating>();

        foreach (var playerMineRating in usersRatings)
        {
            ratingList.Add(playerMineRating);
        }

        _data.PvpTop = ratingList;

    }

    private void CalculateAllMineRating()
    {
        var usersRatings = _data.MineTop;

        usersRatings.Sort((a, b) =>
        {
            if (a.Prestige > b.Prestige)
                return -1;

            if (a.Prestige < b.Prestige)
                return 1;

            if (a.Tier > b.Tier)
                return -1;

            if (a.Tier < b.Tier)
                return 1;

            if (a.Mine > b.Mine)
                return -1;

            if (a.Mine < b.Mine)
                return 1;

            var aRating = 0;
            aRating += a.Rating;
            aRating += (a.HardCoreRating * 2);

            var bRating = 0;
            bRating += b.Rating;
            bRating += (b.HardCoreRating * 2);

            if (aRating > bRating)
                return -1;

            return aRating < bRating ? 1 : 0;
        });

        var ratingList = new List<PlayerMineRating>();
        foreach (var playerMineRating in usersRatings)
        {
            ratingList.Add(playerMineRating);
        }

        _data.MineTop = ratingList;

    }



    public void Calculate()
    {
        if (TimeManager.Instance.NowUnixSeconds < _data.NextUpdate)
            return;

        Debug.Log("Calculate Rating");

        _data.NextUpdate = TimeManager.Instance.NowUnixSeconds + _updatePeriod;
        

        // Mix bots 
        foreach (var dataPlayer in _data.Players)
        {
            if (App.Instance.Player.Id != dataPlayer.Id)
            {
                var staticBot = App.Instance.StaticData.PvpBots.Find(x => x.Id == dataPlayer.Id);

                if (staticBot == null) continue;

                var random = UnityEngine.Random.Range(staticBot.MinRatingOffset,
                    staticBot.MaxRatingOffset);

                var pvp = _data.PvpTop.Find(x => x.UserId == staticBot.Id && x.UserNick == dataPlayer.Name);

                if (pvp != null)
                {
                    pvp.Rating = random;
                    pvp.League = PvpHelper.GetLeagueByRating(staticBot.MaxRatingOffset);
                }

                var mine = _data.MineTop.Find(x => x.UserId == staticBot.Id && x.UserNick == dataPlayer.Name);

                if (mine != null)
                {
                    mine.HardCoreRating = UnityEngine.Random.Range(0, 5000);
                    mine.Mine = UnityEngine.Random.Range(1, 6);
                    mine.Tier = UnityEngine.Random.Range(1, 49);
                    mine.Prestige = UnityEngine.Random.Range(0, 6);
                }
            }
        }


        CalculateAllMinePvpRating();
        CalculateAllMineRating();
        CalculateAllNewbieRating();

        Debug.Log("Calculate done");

        Save();
    }


    public Player GetPlayer(string userId)
    {
        return _data.Players.Find(x => x.Id == userId);
    }

    public void AddTimeAndCalc()
    {
        TimeManager.Instance.T += 60 * 60 * 24 * 30;
       Initialize();
    }
    private PlayerMineRating GetPlayerMineRating(Player bot)
    {
        if (bot == null) return null;

        return _data.MineTop.Find(x => x.UserId == bot.Id && x.UserNick == bot.Name);
    }
    private PlayerMineRating GetPlayerNewbieRating(Player bot)
    {
        if (bot == null) return null;

        return _data.NewBieTop.Find(x => x.UserId == bot.Id && x.UserNick == bot.Name);
    }
    public PlayerPvpRating GetPlayerPvpRating(Player bot)
    {
        if (bot == null) return null;

        return _data.PvpTop.Find(x => x.UserId == bot.Id && x.UserNick == bot.Name);
    }

}
