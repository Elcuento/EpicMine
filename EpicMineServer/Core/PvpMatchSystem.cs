using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AMTServerDLL;
using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using EpicMineServerDLL.Static.Enums;
using EpicMineServerDLL.Static.Helpers;
using MongoDB.Driver;
using Player = AMTServer.Dto.Player;
using PvpHelper = AMTServer.Helpers.PvpHelper;

namespace AMTServer.Core
{
    public class PvpMatchSystem : IDisposable
    {
        private DataBaseLinks _links;

        private FileSystem _fileArchive;

        private string _path;

        private Thread _updateThread;

        private List<PvpArenaMatchInfo> _matches;

        private EpicMineServer _server;

        public int MatchCount;

        private object _lockObj = new object();

        protected List<PvpArenaMatchInfo> Matches
        {
            get
            {
                lock (_matches)
                {
                    return _matches;
                }
            }
        }


        public PvpMatchSystem(string path, FileSystem fileArchive, EpicMineServer server, DataBaseLinks links)
        {
            _path = path + "\\Matches\\";
            _links = links;

            _fileArchive = fileArchive;
            _server = server;

            _matches = new List<PvpArenaMatchInfo>();
           
        }

        private void Update()
        {
            while (true)
            {
                Thread.Sleep(1000);

                var timeNow = Utils.GetUnixTime();

                try
                {
                    lock (_lockObj)
                    {
                        MatchCount = Matches.Count;

                        for (var index = 0; index < Matches.Count; index++)
                        {
                            var pvpArenaMatchInfo = Matches[index];

                            if (pvpArenaMatchInfo.Status == PvpArenaMatchStatusType.Started &&
                                IsMatchEnd(pvpArenaMatchInfo, timeNow))
                            {
                                EndMatch(pvpArenaMatchInfo);
                                continue;
                            }

                            if (IsTimeOut(pvpArenaMatchInfo, timeNow) ||
                                IsEmpty(pvpArenaMatchInfo, timeNow) ||
                                pvpArenaMatchInfo.IsConfirmed())
                            {
                                EndMatch(pvpArenaMatchInfo);
                                Matches.Remove(pvpArenaMatchInfo);
                                index--;
                            }
                        }
                    }

                    SaveMatches();
                }
                catch (Exception e)
                {
                   Log(e.ToString(),true);
                }
            }
        }

        public bool IsTimeOut(PvpArenaMatchInfo info, long timeNow)
        {
            if (info.TimeOutTime < timeNow)
                return true;

            return false;
        }

        public bool IsEmpty(PvpArenaMatchInfo info, long timeNow)
        { 
            if (info.Players.Count == 0)
                return true;

            return false;
        }

        public bool IsMatchEnd(PvpArenaMatchInfo info, long timeNow)
        {
            if (info.Status == PvpArenaMatchStatusType.End)
                return true;


            if (info.EndTime < timeNow
                || info.Players.Find(x => x.Walls >= PvpConstrains.MatchArenaWalls) != null)
                return true;

            var userPlayed = 0;

            foreach (var pvpArenaUserInfo in info.Players)
            {
                if (!info.IsConfirmed(pvpArenaUserInfo.Id))
                    userPlayed++;

            }

            if (userPlayed == 0 || userPlayed == 1)
                return true;

            return false;
        }

        public void EndMatch(PvpArenaMatchInfo info)
        {
            if (info.Status == PvpArenaMatchStatusType.End)
                return;

            try
            {

                PvpHelper.EndPvpMatch(_links, info, _fileArchive.StaticData);

                info.Status = PvpArenaMatchStatusType.End;

                foreach (var user in info.Players)
                {
                    if (!user.IsBot)
                    {
                        var peer = _server.GetClientById(user.Id);

                        peer?.UpdatePvpStats();
                    }
                }

                BroadCastPvpArenaUpdate(info);
            }
            catch (Exception e)
            {
               Log(e);
            }

            SaveMatches();

            Log("Match ended " + info.Id);
        }

        public void Log(Exception e)
        {
            LogSystem.Log("[PvpMatchSystem]" + e, true);
        }

        public void Log(string str, bool isError = false)
        {
            LogSystem.Log("[PvpMatchSystem]" + str);
        }

        public bool CreateMatch(PvpArenaMatchInfo info)
        {
            info.Status = PvpArenaMatchStatusType.Lobby;

            if (Matches.Find(x => x.Id == info.Id) != null)
                return false;

            lock (_lockObj)
            {
                Matches.Add(info);
            }

            SaveMatches();

            return true;
        }

        public bool StartMatch(PvpArenaMatchInfo info)
        {
            if (info.Status == PvpArenaMatchStatusType.End)
                return false;

            LogSystem.Log("Match start " + info.Id);
            info.TimeOutTime = Utils.GetUnixTime() + 60 * 60;
            info.EndTime = Utils.GetUnixTime() + PvpConstrains.MatchArenaTime + PvpConstrains.MatchArenaStartTime;
            info.MatchTime = PvpConstrains.MatchArenaTime;
            info.Locked = true;
            info.Status = PvpArenaMatchStatusType.Started;

            SaveMatches();

            return true;
        }
    

        public PvpArenaMatchInfo GetMyMatch(string userId)
        {
           return Matches.Find(x => x.Players.Find(y=> y.Id == userId) != null && !x.IsConfirmed(userId));
        }

        public PvpArenaMatchInfo GetMatch(string matchId)
        {
            return Matches.Find(x => x.Id == matchId && x.Status != PvpArenaMatchStatusType.End);
        }

        public bool UpdateMatchUser(PvpArenaUserInfo user)
        {
            var match = GetMyMatch(user?.Id);
            if (match != null)
            {
                var contain = match.Players.Find(x => x.Id == user?.Id);
                if (contain != null)
                {
                    match.Players.Remove(contain);
                    match.Players.Add(user);
                    SaveMatches();
                    BroadCastPvpArenaPlayerInfoUpdate(match, user);
                }

                return true;
            }
            else
            {
                Log("match is null");
                return false;
            }
        }

        public bool UpdateMatch(PvpArenaMatchInfo info)
        {
            var match = Matches.Find(x => x.Id == info.Id);

            if (match != null)
            {
                lock (_lockObj)
                {
                    Matches.Remove(match);
                    Matches.Add(info);
                }

                SaveMatches();
                BroadCastPvpArenaUpdate(match);
                return true;
            }

            else return false;
        }

        private void SaveMatches()
        {
            lock (_lockObj)
            {
                File.WriteAllText(_path + "pvpMatchSystem.txt", Matches.ToJson());
            }
        }

        private void LoadMatches()
        {

            try
            {  
                _matches = File.ReadAllText(_path + "pvpMatchSystem.txt").FromJson<List<PvpArenaMatchInfo>>() ??
                           new List<PvpArenaMatchInfo>();
            }
            catch (Exception e)
            {
               Log("No matches files, create new");
                _matches = new List<PvpArenaMatchInfo>();
            }

         //   var timeNow = Utils.GetUnixTime();

          /*  for (var index = 0; index < Matches.Count; index++)
            {
                var match = Matches[index];

                if (IsMatchEnd(match, timeNow))
                {
                    EndMatch(match);
                }
                else
                {
                    Matches.Add(match);
                }
            }*/

           // _links.UserPvpMatchCollection.DeleteMany(Builders<Dto.PvpArenaMatch>.Filter.Empty);
        }

        public void PostInitialize()
        {
            LoadMatches();

            _updateThread = new Thread(Update);
            _updateThread.Start();
        }

        public void Dispose()
        {
            _updateThread?.Abort();
        }


        public void Leave(PvpArenaMatchInfo match, string userId)
        {
            if (match == null)
                return;

            var player = match.Players.Find(x => x.Id == userId);

            if (player == null || match.IsConfirmed(userId))
                return;

            match.Confirm(userId);

            if (match.Status != PvpArenaMatchStatusType.Started 
                && match.Status != PvpArenaMatchStatusType.End)
            {
                match.Players.Remove(player);
                SaveMatches();
                BroadCastPvpArenaUpdate(match);
            }

        }

        public PvpArenaUserInfo SetBot(PvpArenaMatchInfo arena, Player player)
        {
            if (arena != null)
            {
                if (arena.Players.Count == 1
                    && arena.Players.Find(x => x.Id == player.Data.Id) != null
                    && !arena.Locked
                    && arena.Status != PvpArenaMatchStatusType.Started
                    && arena.Status != PvpArenaMatchStatusType.End)
                {

                    var botData = PvpHelper.GetPvpBotAccordingLeague(player, _fileArchive.StaticData);
                    if (botData == null)
                    {
                        Log("cant find bot for player ",true);
                        return null;
                    }

                    var bot = _links.UserPvpRatingCollection.FindSync(x => x.Rating.UserId == botData.Id)
                        .FirstOrDefault();

                    if (bot == null)
                    {
                        Log("cant find rating bot for player ",true);
                        return null;
                    }

                    var botUser = new PvpArenaUserInfo
                    {
                        Id = botData.Id,
                        Rating = bot.Rating.Rating,
                        IsBot = true,
                        Localate = bot.Rating.UserLocalate,
                        Name = bot.Rating.UserNick,
                        Pickaxe = PvpHelper.GetBotRandomPickAxe(player, _fileArchive.StaticData, botData)?.Id,
                        Torch = PvpHelper.GetRandomTorch(player, _fileArchive.StaticData, botData)?.Id,
                };

                    if (!AddPlayer(arena, botUser))
                    {
                        Log("cant add bot, its already in",true);
                        return null;
                    }

               
                    return botUser;
                }

                return null;
            }
            else
            {
                Log("Arena not exist", true);
                return null;
            }
        }

        public void BroadCastPvpArenaSendEmoji(PvpArenaMatchInfo match, int id)
        {
            foreach (var player in match.Players)
            {
                if (player.IsBot)
                    continue;

                if (match.IsConfirmed(player.Id))
                    continue;

                _server.SendResponseLessMessageToOther(
                    player.Id, new ResponseDataPvpSendEmoji(match.Id, id), CommandType.PvpGetSendEmoji);
            }
        }

        public void BroadCastPvpArenaUpdate(PvpArenaMatchInfo match)
        {
            foreach (var player in match.Players)
            {
                if (player.IsBot)
                    continue;

                if(match.IsConfirmed(player.Id))
                    continue;

                _server.SendResponseLessMessageToOther(
                    player.Id, new ResponseDataPvpUpdateMatchInfo(match), CommandType.PvpGetUpdateMatchInfo);
            }
        }

        public void BroadCastPvpArenaPlayerInfoUpdate(PvpArenaMatchInfo match, PvpArenaUserInfo user)
        {
            foreach (var player in match.Players)
            {
                if (player.IsBot)
                    continue;

                if (match.IsConfirmed(player.Id))
                    continue;

                _server.SendResponseLessMessageToOther(
                    player.Id, new ResponseDataPvpUpdatePlayerInfo(user), CommandType.PvpGetUpdateUserInfo);
            }
        }
        public PvpArenaUserInfo AddPlayer(PvpArenaMatchInfo info, string userId)
        {
            var match = Matches.Find(x => x.Id == info.Id);

 
            if (match != null && !match.Locked && !match.IsFull() && !match.HasPlayer(userId))
            {
                var client = _server.GetClientById(userId);

                if (client?.Player?.Data == null)
                    return null;

                var userInfo = new PvpArenaUserInfo(client.Player.Data);

                match.AddPlayer(userInfo);

                if (match.Players.Count == match.MaxPlayers)
                {
                    StartMatch(match);
                }

                SaveMatches();
                BroadCastPvpArenaUpdate(match);

                return userInfo;
            }

            return null;
        }

        public bool AddPlayer(PvpArenaMatchInfo info, PvpArenaUserInfo user)
        {

            var match = Matches.Find(x => x.Id == info.Id);

            if (match != null && !match.Locked && !match.IsFull() && !match.HasPlayer(user.Id))
            {
                match.AddPlayer(user);

                if (match.Players.Count == match.MaxPlayers)
                {
                    StartMatch(match);
                }

                SaveMatches();
                BroadCastPvpArenaUpdate(match);

                return true;
            }

            return false;
        }

        public PvpArenaMatchInfo CreatePvpArena(Player player, int arena)
        {
            var staticData = _fileArchive.StaticData;

            var ratingMax = player.Data.Pvp.Rating + 200;
            var ratingMin = player.Data.Pvp.Rating <= 200
                ? 0
                : player.Data.Pvp.Rating - 200;

            var leagueSettingsNumber = EpicMineServerDLL.Static.Helpers.PvpHelper.GetSuiteSettingsNumber(staticData, player.Data.Pvp.Rating);

            var filter = new PvpArenaMatchFilter
            {
                Prestige = player.Data.Prestige,
                MinRating = ratingMin,
                MaxRating = ratingMax,
                LeagueSettingsNumber = leagueSettingsNumber,
                MaxUsers = 2
            };


            var tier = player.Data.Dungeon.LastOpenedTier?.Number ?? 0;
            var walls = EpicMineServerDLL.Static.Helpers.PvpHelper.GetWallsHealths(staticData, tier, leagueSettingsNumber, 8);

            var data = new PvpArenaMatchInfo(filter)
            {
                MaxPlayers = 2,
                Players = new List<PvpArenaUserInfo>(),
                Walls = walls,
                Type = PvpArenaMatchType.Duel,
                Arena = arena,
                TimeOutTime = Utils.GetUnixTime() + 60 * 60 * 24
            };

            if (!CreateMatch(data))
                return null;

            var user = AddPlayer(data, player.Data.Id);

            if (user == null)
                return null;

            return data;
        }

        public PvpArenaMatchInfo JoinCreateArena(Player player, int arena)
        {
            var staticData = _fileArchive.StaticData;

            var ratingMax = player.Data.Pvp.Rating + 200;
            var ratingMin = player.Data.Pvp.Rating <= 200
                ? 0
                : player.Data.Pvp.Rating - 200;

            var leagueSettingsNumber = EpicMineServerDLL.Static.Helpers.PvpHelper.GetSuiteSettingsNumber(staticData, player.Data.Pvp.Rating);

            var filter = new PvpArenaMatchFilter
            {
                Prestige = player.Data.Prestige,
                MinRating = ratingMin,
                MaxRating = ratingMax,
                LeagueSettingsNumber = leagueSettingsNumber,
                MaxUsers = 2
            };


            var tier = player.Data.Dungeon.LastOpenedTier?.Number ?? 0;
            var walls = EpicMineServerDLL.Static.Helpers.PvpHelper.GetWallsHealths(staticData, tier, leagueSettingsNumber, 8);

            var data = Matches.Find(x => !x.Locked && !x.IsFull() && 
                                         EpicMineServerDLL.Static.Helpers.PvpHelper.SuiteFilter(x.Filter,filter));

            if (data == null)
            {
                data = new PvpArenaMatchInfo(filter)
                {
                    MaxPlayers = 2,
                    Players = new List<PvpArenaUserInfo>(),
                    Walls = walls,
                    Type = PvpArenaMatchType.RandomMatch,
                    Arena = arena,
                    TimeOutTime = Utils.GetUnixTime() + 60 * 60 * 24
            };

                if (!CreateMatch(data))
                    return null;
            }

            var user = AddPlayer(data, player.Data.Id);

            if (user == null)
                return null;

            return data;
        }

        public void Confirm(PvpArenaMatchInfo arena, string dataId)
        {
            lock (_lockObj)
            {
                var match = Matches.Find(x => x.Id == arena.Id);

                if (match.Players.Find(x => x.Id != dataId) != null)
                {
                    match.Confirm(dataId);
                }

            }

            SaveMatches();
        }

        public void SendEmoji(string valueId, int i)
        {
            lock (_lockObj)
            {
                var match = Matches.Find(x => x.Id == valueId);

                if (match != null)
                {
                    BroadCastPvpArenaSendEmoji(match,i);
                }

            }
        }
    }
}
