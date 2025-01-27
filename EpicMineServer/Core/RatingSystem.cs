using System;
using System.Collections.Generic;
using System.Threading;
using AMTServer.Dto;
using AMTServerDLL;
using EpicMineServerDLL.Static.Helpers;
using MongoDB.Driver;
using PlayerMineRating = CommonDLL.Dto.PlayerMineRating;

namespace AMTServer.Core
{
    public class RatingSystem
    {
        private readonly int _updatePeriod = 60 * 60 * 12;

        public DateTime LastTimeUpdate { get; private set; }
        public DateTime NextUpdateTime { get; private set; }
        public long LastCalculateSpend { get; private set; }
        public bool IsUpdating { get; private set; }

        private RatingInfo _data;

        private DataBaseLinks _links;

        private FileSystem _fileSystem;

        private EpicMineServer _server;

        private System.Timers.Timer _ratingTimer;

        public RatingSystem(EpicMineServer server, FileSystem fileSystem, DataBaseLinks links)
        {
            _server = server;

            _links = links;
            _fileSystem = fileSystem;

            _data = new RatingInfo();

            try
            {
                GetData();

            }
            catch (Exception e)
            {
                LogSystem.Log("Rating load error" + e);

            }

            if (_data == null)
            {
                _data = new RatingInfo();
                LogSystem.Log("Create rating");
            }

            LastTimeUpdate = Common.Utils.FromUnix(_data.LastUpdate);
            LastCalculateSpend = _data.Spend;
            NextUpdateTime = Common.Utils.FromUnix(_data.LastUpdate + _updatePeriod);

            CreateTimer(_data.LastUpdate + _updatePeriod - Utils.GetUnixTime());
        }

        public void Log(Exception e)
        {
            LogSystem.Log("[RatingSystem]" + e, true);
        }

        public void Log(string str, bool isError = false)
        {
            LogSystem.Log("[RatingSystem]" + str, isError);
        }

        public void CreateTimer(long timeLeft)
        {
            _ratingTimer = new System.Timers.Timer();

            _ratingTimer.Elapsed += (b, s) =>
            {
                if (!_server.IsLoginServer)
                {
                    GetData();
                }
                else
                {
                    Calculate();
                }

                _ratingTimer.Interval = _updatePeriod * 1000;
            };


            _ratingTimer.Interval = timeLeft > 0 ? timeLeft * 1000 : 1000;
            _ratingTimer.Enabled = true;
        }

        public void GetData()
        {
            lock (_data)
            {
                _data = _links.RatingInfoCollection.FindSync(Builders<RatingInfo>.Filter.Empty).FirstOrDefault() ?? new RatingInfo();

                if (_data == null)
                {
                    _data = new RatingInfo();
                    return;
                }

                if (_data.MineTop == null)
                    _data.MineTop = new List<Dto.PlayerMineRating>();

                if (_data.NewBieTop == null)
                    _data.NewBieTop = new List<Dto.PlayerMineRating>();

                if (_data.PvpTop == null)
                    _data.PvpTop = new List<PlayerPvpRating>();
            }

        }

        public void PostInitialize()
        {
            if (!_server.IsLoginServer)
                return;

            LoadBots();
        }

        public void LoadBots()
        {
            var staticData = _fileSystem.GetStaticDataCopy();

            if (staticData != null)
            {
                foreach (var staticDataPvpBot in staticData.PvpBots)
                {
                    foreach (var botName in staticDataPvpBot.Names)
                    {
                        var name = botName + PvpConstrains.BotNamePrefix;
                        var bot = _links.UserPvpRatingCollection
                            .FindSync(x => x.Rating.UserId == staticDataPvpBot.Id && x.Rating.UserNick == name)
                            .FirstOrDefault();

                        if (bot == null)
                        {
                            var random = new Random((int) DateTime.UtcNow.Ticks).Next(staticDataPvpBot.MinRatingOffset,
                                staticDataPvpBot.MaxRatingOffset);

                            _links.UserPvpRatingCollection.InsertOne(new PlayerPvpRating(
                                new CommonDLL.Dto.PlayerPvpRating
                                {
                                    Rating = random,
                                    UserId = staticDataPvpBot.Id,
                                    UserLocalate = PvpConstrains.CountryCodes[
                                        new Random((int) DateTime.Now.Ticks).Next(0, PvpConstrains.CountryCodes.Count)],
                                    IsBot = true,
                                    UserNick = name,
                                    League = PvpHelper.GetLeagueNumberByRating(staticData,
                                        staticDataPvpBot.MaxRatingOffset),
                                }));
                        }

                    }

                }
            }
        }

        public List<CommonDLL.Dto.PlayerPvpRating> GetPvpRating()
        {
            lock (_data)
            {
                if (_data.PvpTop.Count <= 0)
                    return new List<CommonDLL.Dto.PlayerPvpRating>();
                else
                {
                    var max = _data.PvpTop.Count <= 100 ? _data.PvpTop.Count : 100;

                    var ratingList = _data.PvpTop.GetRange(0, max);
                    var rating = new List<CommonDLL.Dto.PlayerPvpRating>();

                    for (var index = 0; index < ratingList.Count; index++)
                    {
                        var playerMineRating = ratingList[index];
                        rating.Add(playerMineRating.Rating);
                    }

                    return rating;
                }
            }
        }

        public List<PlayerMineRating> GetMineRating()
        {
            lock (_data)
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
                        rating.Add(playerMineRating.Rating);
                    }

                    return rating;
                }
            }
        }

        public List<PlayerMineRating> GetNewBieMineRating()
        {
            lock (_data)
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
                        rating.Add(playerMineRating.Rating);
                    }

                    return rating;
                }
            }
        }

        private void CalculateNewbieRating()
        {
            var usersRatings = _links.UserMineRatingCollection.FindSync(Builders<Dto.PlayerMineRating>.Filter.Empty)
                .ToList();

            var idList = new List<string>();
            foreach (var playerMineRating in usersRatings)
            {
                idList.Add(playerMineRating.Rating.UserId);
            }

            var users = _links.UserCollection.FindSync(x => idList.Contains(x.Data.Id)).ToList();

            var endDate = Utils.GetUnixTime() - 30 * 24 * 60 * 60;

            for (var index = 0; index < usersRatings.Count; index++)
            {
                var playerMineRating = usersRatings[index];
                var user = users.Find(x => x.Data.Id == playerMineRating.Rating.UserId);

                if (user == null || user.Data.CreationDate < endDate)
                {
                    usersRatings.Remove(playerMineRating);
                    index--;
                }
            }

            usersRatings.Sort((a, b) =>
            {
                if (a.Rating.Prestige > b.Rating.Prestige)
                    return -1;

                if (a.Rating.Prestige < b.Rating.Prestige)
                    return 1;

                if (a.Rating.Tier > b.Rating.Tier)
                    return -1;

                if (a.Rating.Tier < b.Rating.Tier)
                    return 1;

                if (a.Rating.Mine > b.Rating.Mine)
                    return -1;

                if (a.Rating.Mine < b.Rating.Mine)
                    return 1;

                var aRating = 0;
                aRating += a.Rating.Rating;
                aRating += (a.Rating.HardCoreRating * 2);

                var bRating = 0;
                bRating += b.Rating.Rating;
                bRating += (b.Rating.HardCoreRating * 2);

                if (aRating > bRating)
                    return -1;

                return aRating < bRating ? 1 : 0;
            });

            var ratingList = new List<Dto.PlayerMineRating>();
            foreach (var playerMineRating in usersRatings)
            {
                ratingList.Add(playerMineRating);
            }

            lock (_data)
            {
              _data.NewBieTop = ratingList;
            }
        }

        private void ClearMinePvpRatingForOldUsers()
        {
            var usersRatings = _links.UserPvpRatingCollection.FindSync(Builders<Dto.PlayerPvpRating>.Filter.Empty)
                .ToList();

            var idList = new List<string>();
            foreach (var playerMineRating in usersRatings)
            {
                if (playerMineRating.Rating.IsBot)
                    continue;

                idList.Add(playerMineRating.Rating.UserId);
            }

            var users = _links.UserCollection.FindSync(x => idList.Contains(x.Data.Id)).ToList();

            var endDate = Utils.GetUnixTime() - 30 * 24 * 60 * 60;

            for (var index = 0; index < usersRatings.Count; index++)
            {
                var playerMineRating = usersRatings[index];
                var user = users.Find(x => x.Data.Id == playerMineRating.Rating.UserId);

                if (user == null || user.LastOnlineDate < endDate)
                {
                    _links.UserPvpRatingCollection.DeleteOne(x => x.Id == playerMineRating.Id);
                }
            }
        }

        private void ClearMineRatingForOldUsers()
        {
            var usersRatings = _links.UserMineRatingCollection.FindSync(Builders<Dto.PlayerMineRating>.Filter.Empty)
                .ToList();

            var idList = new List<string>();
            foreach (var playerMineRating in usersRatings)
            {
                idList.Add(playerMineRating.Rating.UserId);
            }

            var users = _links.UserCollection.FindSync(x => idList.Contains(x.Data.Id)).ToList();

            var endDate = Utils.GetUnixTime() - 30 * 24 * 60 * 60;

            for (var index = 0; index < usersRatings.Count; index++)
            {
                var playerMineRating = usersRatings[index];
                var user = users.Find(x => x.Data.Id == playerMineRating.Rating.UserId);

                if (user == null || user.LastOnlineDate < endDate)
                {
                    _links.UserMineRatingCollection.DeleteOne(x => x.Id == playerMineRating.Id);
                }
            }
        }

        private void CalculateMinePvpRating()
        {

            var usersRatings = _links.UserPvpRatingCollection.FindSync(Builders<PlayerPvpRating>.Filter.Empty)
                .ToList();

            usersRatings.Sort((a, b) =>
            {

                var aRating = 0;
                aRating += a.Rating.Rating;

                var bRating = 0;
                bRating += b.Rating.Rating;

                if (aRating > bRating)
                    return -1;

                return aRating < bRating ? 1 : 0;

            });

            var ratingList = new List<PlayerPvpRating>();
            foreach (var playerMineRating in usersRatings)
            {
                ratingList.Add(playerMineRating);
            }

            lock (_data)
            {
                _data.PvpTop = ratingList;
            }


        }

        private void CalculateMineRating()
        {
            var usersRatings = _links.UserMineRatingCollection.FindSync(Builders<Dto.PlayerMineRating>.Filter.Empty)
                .ToList();

            usersRatings.Sort((a, b) =>
            {
                if (a.Rating.Prestige > b.Rating.Prestige)
                    return -1;

                if (a.Rating.Prestige < b.Rating.Prestige)
                    return 1;

                if (a.Rating.Tier > b.Rating.Tier)
                    return -1;

                if (a.Rating.Tier < b.Rating.Tier)
                    return 1;

                if (a.Rating.Mine > b.Rating.Mine)
                    return -1;

                if (a.Rating.Mine < b.Rating.Mine)
                    return 1;

                var aRating = 0;
                aRating += a.Rating.Rating;
                aRating += (a.Rating.HardCoreRating * 2);

                var bRating = 0;
                bRating += b.Rating.Rating;
                bRating += (b.Rating.HardCoreRating * 2);

                if (aRating > bRating)
                    return -1;

                return aRating < bRating ? 1 : 0;
            });

            var ratingList = new List<Dto.PlayerMineRating>();
            foreach (var playerMineRating in usersRatings)
            {
                ratingList.Add(playerMineRating);
            }

            lock (_data)
            {
                _data.MineTop = ratingList;
            }

        }



        public void Calculate()
        {
            Log("Calculate ratings");
            LastTimeUpdate = DateTime.Now;
            NextUpdateTime = Common.Utils.FromUnix(Utils.GetUnixTime() + _updatePeriod);

            var timeStart = Utils.GetUnixTime();
            var calculateThread = new Thread(() =>
            {
                IsUpdating = true;
                try
                {
                    ClearMineRatingForOldUsers();
                    ClearMinePvpRatingForOldUsers();
                    Log("Calculate clear old, spend " + Common.Utils.GetTime((Utils.GetUnixTime() - timeStart)));

                    CalculateMinePvpRating();
                    Log("Calculate pvp, spend " + Common.Utils.GetTime((Utils.GetUnixTime() - timeStart)));
                    CalculateMineRating();
                    Log("Calculate mine, spend " + Common.Utils.GetTime((Utils.GetUnixTime() - timeStart)));
                    CalculateNewbieRating();
                    Log("Calculate done" + Common.Utils.GetTime((Utils.GetUnixTime() - timeStart)));

                }
                catch (Exception e)
                {
                    LogSystem.Log("Calculate error " + e);
                }

                IsUpdating = false;
                LastCalculateSpend = Utils.GetUnixTime() - timeStart;
                SaveFile();


                Log("Calculate done , spend " + Common.Utils.GetTime(LastCalculateSpend));
            });

            calculateThread.Start();
        }

        private void SaveFile()
        {
            lock (_data)
            {
                _data.LastUpdate = Utils.GetUnixTime();
                _data.Spend = LastCalculateSpend;

                _links.RatingInfoCollection.DeleteMany(Builders<Dto.RatingInfo>.Filter.Empty);
                _links.RatingInfoCollection.InsertOne(_data);
            }

        }

        public void Dispose()
        {
            _ratingTimer?.Dispose();
        }

    }
}
