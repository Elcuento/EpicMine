using System;
using System.Collections.Generic;
using BlackTemple.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;


namespace BlackTemple.EpicMine.Core
{
    public class Pvp
    {
        public const int MatchArenaTime = 180;
        public const int MatchArenaStartTime = 3;
        public const int MatchArenaWalls = 8;

        public static readonly List<string> CountryCodes = new List<string>()
        {
            {"ZA"},
            {"SA"},
            {"US"},
            {"BY"},
            {"BJ"},
            {"ES"},
            {"CN"},
            {"HK"},
            {"DK"},
            {"BE"},
            {"EN"},
            {"EE"},
            {"FU"},
            {"FI"},
            {"FR"},
            {"DE"},
            {"JR"},
            {"IL"},
            {"IS"},
            {"ID"},
            {"IT"},
            {"JP"},
            {"KR"},
            {"LV"},
            {"LT"},
            {"NO"},
            {"PL"},
            {"PT"},
            {"RO"},
            {"RU"},
            {"SP"},
            {"SK"},
            {"SI"},
            {"ES"},
            {"SE"},
            {"TH"},
            {"TR"},
            {"UA"},
            {"VN"},
            {"CN"},
            {"CN"},
            {"US"},
            {"HU"},
        };
        public int Rating => _rating;
        public int Chests => _chests;

        public int Win { get;  set; }
        public int Loose { get;  set; }
        public int Games { get;  set; }
        public bool InviteDisable { get; private set; }
        public List<LastTimePlayed> LastTimePlayed { get; set; }

        public ObscuredInt _rating;
        public ObscuredInt _chests;

        public League CurrentLeague => App.Instance.StaticData.Leagues[PvpHelper.GetLeagueByRating(Rating)];
        public int CurrentLeagueLvl => PvpHelper.GetLeagueByRating(Rating);


        public Pvp(CommonDLL.Dto.Pvp dungeonGameDataResponse)
        {
            _rating = dungeonGameDataResponse.Rating;
            _chests = dungeonGameDataResponse.Chests;

            Win = dungeonGameDataResponse.Win;
            Loose = dungeonGameDataResponse.Loose;
            Games = dungeonGameDataResponse.Games;
            InviteDisable = dungeonGameDataResponse.InviteDisable;

            LastTimePlayed = new List<LastTimePlayed>();
            if (dungeonGameDataResponse.LastTimePlayed != null)
            {
                foreach (var lastTimePlayed in dungeonGameDataResponse.LastTimePlayed)
                {
                    LastTimePlayed.Add(new LastTimePlayed(lastTimePlayed.PlayerName));
                }
            }

            EventManager.Instance.Subscribe<PvpUpdateChangeEvent>(Update);
            EventManager.Instance.Subscribe<PvpArenaAcceptInviteEvent>(OnGetAcceptInvite);
            EventManager.Instance.Subscribe<PvpArenaGetInvitedEvent>(OnGetInvite);
            EventManager.Instance.Subscribe<PvpArenaGetCancelInviteEvent>(OnGetInviteCancel);
            EventManager.Instance.Subscribe<PvpArenaGetDeniedInviteEvent>(OnGetInviteDenied);

        }

        public void Update(PvpUpdateChangeEvent eventData)
        {
            var data = eventData.PvpData;

            _rating = data.Rating;
            _chests = data.Chests;

            Win = data.Win;
            Loose = data.Loose;
            Games = data.Games;
            InviteDisable = data.InviteDisable;

            LastTimePlayed = new List<LastTimePlayed>();
            if (data.LastTimePlayed != null)
            {
                foreach (var lastTimePlayed in data.LastTimePlayed)
                {
                    LastTimePlayed.Add(new LastTimePlayed(lastTimePlayed.PlayerName));
                }
            }
        }

        public void SetChests(int val)
        {
            _chests = val >= 0 && val <= PvpLocalConfig.PvpWinChestRequire ? val : 0;
        }

        public void SetInviteState(bool isEnable)
        {
            InviteDisable = isEnable;
            EventManager.Instance.Publish(new PvpInviteEnableEvent(isEnable));
        }

        /* public void EndLastMatch(PvpArenaGameResoultType result, int opponentRating, Action onComplete, Action onFailed)
         {
             var currentRatingChange = 0;
             var currentOpponentRatingChange = 0;
 
             var request = new EndPvpMatchRequest(result, PvpArenaMatchType.RandomMatch, Player.Nickname, opponentRating);
             NetworkManager.Instance.Send<EndPvpMatchResponse>(
                 request,
                 completeResponse =>
                 {
                     currentOpponentRatingChange = completeResponse.ResultOpponentRating - opponentRating;
                     currentRatingChange = completeResponse.ResultRating - Rating;
 
                     _rating = completeResponse.ResultRating;
                     _chests = completeResponse.ResultChests;
                     Win = completeResponse.ResultWin;
                     Loose = completeResponse.ResultLoose;
                     Games = completeResponse.ResultGames;
 
                     EventManager.Instance.Publish(new PvpUpdateChangeEvent());
 
                     EventManager.Instance.Publish(new PvpArenaEndGameResoultEvent(result, completeResponse.ResultRating,
                         completeResponse.ResultOpponentRating, currentRatingChange,
                         currentOpponentRatingChange));
                
 
                 }, (error) =>
                 {
                 }
             );
 
         }*/

        /* public void EndMatch(PvpArenaGameResoultType result, PvpArenaMatchType matchType, PvpChestType matchChest, bool finished,
             string opponentId, string opponentName, int opponentRating, string opponentPickAxe, bool isBot = false, Action onError = null)
         {
 
             var isGainItem = matchType == PvpArenaMatchType.RandomMatch && result == PvpArenaGameResoultType.Win;
 
             var currentRatingChange = 0;
             var currentOpponentRatingChange = 0;
 
 
             if (!isBot)
             {
                 var request = new EndPvpMatchRequest(result, matchType, Player.Nickname, opponentRating);
                 NetworkManager.Instance.Send<EndPvpMatchResponse>(
                     request,
                     completeResponse =>
                     {
                         currentOpponentRatingChange  = completeResponse.ResultOpponentRating - opponentRating;
                         currentRatingChange = completeResponse.ResultRating - Rating;
 
                         _rating = completeResponse.ResultRating;
                         _chests = completeResponse.ResultChests;
                         Win = completeResponse.ResultWin;
                         Loose = completeResponse.ResultLoose;
                         Games = completeResponse.ResultGames;
 
                         SetLastTimePlayed(opponentName);
 
                         if (isGainItem)
                         {
                             var dtoItem =
                                 new Item(
                                     matchChest == PvpChestType.Royal
                                         ? PvpLocalConfig.PvpRoyalChestItemId
                                         : PvpLocalConfig.PvpSimpleChestItemId, 1);
                             App.Instance.Player.Inventory.Add(dtoItem, IncomeSourceType.FromPvp);
                         }
 
                         EventManager.Instance.Publish(new PvpUpdateChangeEvent());
 
                         EventManager.Instance.Publish(new PvpArenaEndGameResoultEvent(result, completeResponse.ResultRating,
                             completeResponse.ResultOpponentRating, currentRatingChange,
                             currentOpponentRatingChange));
 
                     }, (error) =>
                     {
                         onError?.Invoke();
                     }
                 );
             }
             else
             {
                 var bot = App.Instance.StaticData.PvpBots.FirstOrDefault(x => x.Id == opponentId);
 
                 if (bot != null)
                 {
                     if (bot.MinRatingOffset > opponentRating)
                     {
                         opponentRating = bot.MinRatingOffset;
                     }else if (bot.MaxRatingOffset < opponentRating)
                     {
                         opponentRating = bot.MaxRatingOffset;
                     }
                 }
 
                 var request = new EndPvpMatchWithBotRequest(result, PvpArenaMatchType.RandomMatch, Player.Nickname,opponentName, opponentId, opponentRating);
                 NetworkManager.Instance.Send<EndPvpMatchResponse>(
                     request,
                     completeResponse =>
                     {
                         currentOpponentRatingChange = completeResponse.ResultOpponentRating - opponentRating;
                         currentRatingChange = completeResponse.ResultRating - Rating;
                         _rating = completeResponse.ResultRating;
                         _chests = completeResponse.ResultChests;
                         Win = completeResponse.ResultWin;
                         Loose = completeResponse.ResultLoose;
                         Games = completeResponse.ResultGames;
 
                         if (isGainItem)
                         {
                             var dtoItem =
                                 new Item(
                                     matchChest == PvpChestType.Royal
                                         ? PvpLocalConfig.PvpRoyalChestItemId
                                         : PvpLocalConfig.PvpSimpleChestItemId, 1);
                             App.Instance.Player.Inventory.Add(dtoItem, IncomeSourceType.FromPvp);
                         }
 
                         EventManager.Instance.Publish(new PvpUpdateChangeEvent());
 
                         EventManager.Instance.Publish(new PvpArenaEndGameResoultEvent(result, completeResponse.ResultRating,
                             completeResponse.ResultOpponentRating, currentRatingChange,
                             currentOpponentRatingChange));
 
                     }, 
                     (error) =>
                     {
                         onError?.Invoke();
                     }
                 );
             }
 
 
         }*/

        #region communicate functions

        public void SendAcceptInvite(string inviteId, string matchId, Action onComplete)
        {
           /* AmtNetworkController.Instance.SendNetworkMessage<ResponseDataPvpInviteAccepted>(new RequestDataPvpInviteAccepted(inviteId, matchId),
                CommandType.PvpInviteAccepted, (data) =>
                {
                    PvpArenaNetworkController.SaveMatchData(data.MatchInfo);
                    SceneManager.Instance.LoadScene(ScenesNames.PvpArena);
                }, (b) => { Debug.Log("Match not exist"); });
            */
        }

        public void SendCancelInvite(string id)
        {
            /*AmtNetworkController.Instance.SendNetworkMessage<SendData>(new RequestDataPvpInviteCancel(id),
                CommandType.PvpInviteCancel, (data) => { });*/
        }


        public void SendDeniedInvite(string id)
        {
         //   AmtNetworkController.Instance.SendNetworkMessage<SendData>(new RequestDataPvpInviteDenied(id),
             //   CommandType.PvpInviteDenied, (data) => { });
        }

        public void SendInvite(string userId)
        {
          //  AmtNetworkController.Instance.SendNetworkMessage<SendData>(new RequestDataPvpInvite(userId),
           //     CommandType.PvpInvite, (data) => { });
        }



     /*   public void FindUser(string userName, Action<ResponseDataPvpFindUserByName> onFind, Action onError)
        {
            AmtNetworkController.Instance.SendNetworkMessage<ResponseDataPvpFindUserByName>(new RequestDataPvpFindUser(userName),
                CommandType.PvpFindUserByName, (data) =>
                {
                    onFind?.Invoke(data);
                }, (b) =>
                {
                    onError?.Invoke();
                });
        }
        */
        public void OnGetInviteCancel(PvpArenaGetCancelInviteEvent data)
        {
            WindowManager.Instance.Show<WindowAlert>()
                .Initialize("Invite was cancelled");

            var win = WindowManager.Instance.Get<WindowPvpArenaSendInvitation>();
            if (win != null)
                win.Close();
        }

        public void OnGetInviteDenied(PvpArenaGetDeniedInviteEvent data)
        {
            WindowManager.Instance.Show<WindowAlert>()
                .Initialize("Invite was denied");
        }

        public void OnGetInvite(PvpArenaGetInvitedEvent data)
        {
            if (WindowManager.Instance.IsOpen<WindowPvpArenaGetInvitation>())
            {
                var prevInviter = WindowManager.Instance.Get<WindowPvpArenaGetInvitation>();

                if (!string.IsNullOrEmpty(prevInviter.InviteId))
                    SendCancelInvite(prevInviter.InviteId);
            }

            WindowManager.Instance.Show<WindowPvpArenaGetInvitation>()
                .Initialize(data.UserInfo, data.MatchId);
        }

        public void OnGetAcceptInvite(PvpArenaAcceptInviteEvent data)
        {
            
        }

        public void SetRating(int rating)
        {
            _rating = rating;
           // EventManager.Instance.Publish(new PvpUpdateChangeEvent());
        }

        #endregion

        public void DeniedInvite()
        {
            //AmtNetworkController.Instance.SendNetworkMessage<SendData>(CommandType.PvpLeaveArena, (data) => { });
        }
    }
}