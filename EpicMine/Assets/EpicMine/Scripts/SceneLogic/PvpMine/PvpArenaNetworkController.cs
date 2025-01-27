using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.Diagnostics;
using Player = BlackTemple.EpicMine.Core.Player;
using Random = UnityEngine.Random;

public class PvpArenaNetworkController : MonoBehaviour
{
    public static PvpArenaNetworkController I
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PvpArenaNetworkController>();
            }

            return _instance;
        }
    }

    private static PvpArenaNetworkController _instance;

    [SerializeField] private PvpArenaMatchStatusType _gameState;

    public bool IsConnected => Application.internetReachability != NetworkReachability.NotReachable;
    public bool IsReady => IsConnected &&
                           IsOpponentReady();

    private Dictionary<int, Action> _checkingActions;
    private int _actionCounter;
    private bool _isBotMode;
    private string _lastRoom;
    private int _reconnectTry;

    private Coroutine _botEnableCoroutine;
    private Coroutine _matchStartCoroutine;
    private Coroutine _connectCoroutine;

    [SerializeField] public PvpArenaUserInfo PlayerData;
    [SerializeField] public PvpArenaUserInfo OpponentData;


    protected void Awake()
    {
        OpponentData = new PvpArenaUserInfo();
        PlayerData = new PvpArenaUserInfo
        {
            Id = App.Instance.Player.Id,
            Rating = App.Instance.Player.Pvp.Rating,
            Localate = App.Instance.Player.Location,
            Pickaxe = App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Id,
            Torch = App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.Id
        };

        EventManager.Instance.Subscribe<PvpArenaSendEmodjiEvent>(OnSendEmo);

       
    }

    private void Start()
    {
        var lobby = WindowManager.Instance.Show<WindowPvpArenaLobby>();
        lobby.Initialize(this);
        /* var matchData = GetMatchData();

         var lobby = WindowManager.Instance.Show<WindowPvpArenaLobby>();
         lobby.Initialize(this);

         if (matchData != null && matchData.Status != PvpArenaMatchStatusType.End)
         {
             var player = matchData.GetPlayer(App.Instance.Player.Id);

             if (player != null)
             {
                 OnPlayerUpdate(player);
                 OnMatchDataUpdate(matchData);
             }
             else
             {
                 Debug.Log("No self data in match");
             }
         }
        */
    }

    public void OnClickStartBattle()
    {
        JoinCreateRandom();
    }

    public IEnumerator LateCallBack()
    {
        while (true)
        {
            yield return null;
        }
    }

  /*  public void OnGetData(Package pack)
    {

        switch ((CommandType) pack.Command)
        {
            case CommandType.PvpGetUpdateUserInfo:

                if (pack.Data is ResponseDataPvpUpdatePlayerInfo userData)
                {
                    if (userData.Info.Id == PlayerData.Id)
                    {
                        OnPlayerUpdate(userData.Info);
                        return;
                    }

                    if (OpponentData == null || string.IsNullOrEmpty(OpponentData.Id) ||
                        OpponentData.Id != userData.Info.Id)
                    {
                        UserJoin(userData.Info);
                    }

                    OnOpponentUpdated(userData.Info);
                }

                break;
            case CommandType.PvpGetSendEmoji:

                if (pack.Data is ResponseDataPvpSendEmoji emojiData)
                {
                    OnGetEmo(emojiData?.Id ?? 0);
                }

                break;
            case CommandType.PvpGetUpdateMatchInfo:

                if (pack.Data is ResponseDataPvpUpdateMatchInfo matchData)
                {
                    var player = matchData.Data.GetPlayerApp.Instance.Player.Nickname;

                    if (matchData.Data.IsConfirmed(App.Instance.Player.Nickname) || player == null)
                        return;

                    SaveMatchData(matchData.Data);

                    OnMatchDataUpdate(matchData.Data);
                }

                break;
        }
    }
    */
    public void OnDestroy()
    {
        
        //  AmtNetworkController.Instance.SendNetworkMessage(CommandType.PvpLeaveArena);

        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<PvpArenaSendEmodjiEvent>(OnSendEmo);

         /*   AmtNetworkController.Instance.OnGetDataEvent -= OnGetData;
            AmtNetworkController.Instance.OnConnectedEvent -= OnConnected;
            AmtNetworkController.Instance.OnDisconnectedEvent -= OnDisconnected;*/
        }

        ClearMatchStorage();
    }

    #region Events

    public void StartMatch()
    {
        if (_matchStartCoroutine != null)
            StopCoroutine(_matchStartCoroutine);

        _matchStartCoroutine = StartCoroutine(_startMatchCounter(PvpLocalConfig.DefaultPvpMineVersusTime));
    }


    #endregion

    #region User commands


    public void ClearMatchStorage()
    {
        if (App.Instance != null)
        {
            DestroyMatchDataWithCheck();
        }
    }

    public void ChangeGameState(PvpArenaMatchStatusType state)
    {

        if (_gameState == PvpArenaMatchStatusType.End)
            return;

        var prevState = _gameState;

        _gameState = state;


        if (prevState == _gameState)
            return;

        var matchData = GetMatchData();

        if (matchData == null) return;
        // var ownData = matchData.GetPlayerApp.Instance.Player.Nickname;
        //if (ownData != null)
        //   OnPlayerUpdate(ownData);

        matchData.Status = _gameState;
        
        SaveMatchData(matchData);

        switch (_gameState)
        {
            case PvpArenaMatchStatusType.None:
                break;

            case PvpArenaMatchStatusType.Lobby:
                WindowManager.Instance.Show<WindowPvpArenaVersusScreen>();
                EventManager.Instance.Publish(new PvpArenaStartGameLobbyEvent(matchData));
                break;
            case PvpArenaMatchStatusType.Waiting:
                break;
            case PvpArenaMatchStatusType.Started:
                WindowManager.Instance.Show<WindowPvpArenaPreStart>();
                EventManager.Instance.Publish(new PvpArenaStartGameEvent(matchData));
                StartMatch();
                break;
            case PvpArenaMatchStatusType.End:
              if (matchData.Result != null && matchData.Result.Result.ContainsKey(PlayerData.Id))
                {
                    var playerResult = matchData.Result.Result[PlayerData.Id];
                    var winner = matchData.Result.Result.FirstOrDefault(x => x.Value == PvpArenaGameResoultType.Win).Key;

                    WindowManager.Instance.Show<WindowPvpArenaPreResoult>()
                        .Initialize(playerResult,
                            () =>
                            {
                                var win = WindowManager.Instance.Show<WindowPvpArenaResoult>();
                                win.Initialzie(this,playerResult, PlayerData, OpponentData, matchData.Type);
                                DestroyMatchData();
                            });

                    EventManager.Instance.Publish(new PvpArenaEndGameEvent(string.IsNullOrEmpty(winner) ? "" : winner, playerResult));
                }
                else
                {
                    Debug.LogError("No result data for player");
                    Leave(() =>
                    {
                        SceneManager.Instance.LoadScene(ScenesNames.PvpArena);
                    });

                }
               
                break;
        }
    }

    #region Rpc Commands

    public void OnGetEmo(int number)
    {
        EventManager.Instance.Publish(new PvpArenaGetEmodjiEvent(number));
    }

    public void OnSendEmo(PvpArenaSendEmodjiEvent evenData)
    {
        var matchData = GetMatchData();

        if (matchData != null)
        {
           // AmtNetworkController.Instance.SendNetworkMessage<SendData>(new RequestDataPvpSendEmoji(matchData.Id, evenData.Number),
             //   CommandType.PvpSendEmoji, withPreLoader: false);
        }
    }

    #endregion


    #endregion

    public static void CreateMatchData(PvpArenaMatchInfo data)
    {
        SaveMatchData(data);
    }

    public void DestroyMatchDataWithCheck()
    {

        if (App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.PvpArenaMatchInfo) &&
            !string.IsNullOrEmpty(_lastRoom))
        {
            var data =
                App.Instance.Services.RuntimeStorage.Load<PvpArenaMatchInfo>(RuntimeStorageKeys.PvpArenaMatchInfo);

            // if (data.RoomName == _lastRoom)
            {
                DestroyMatchData();
            }
        }
    }

    public static void DestroyMatchData()
    {
        if (!App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.PvpArenaMatchInfo))
            return;

        App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.PvpArenaMatchInfo);
    }

    public static PvpArenaMatchInfo GetMatchData()
    {
        if (App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.PvpArenaMatchInfo))
        {
            return App.Instance.Services.RuntimeStorage.Load<PvpArenaMatchInfo>(RuntimeStorageKeys.PvpArenaMatchInfo);
        }

        return null;
    }

    public static void SaveMatchData(PvpArenaMatchInfo data)
    {
        App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.PvpArenaMatchInfo, data);
    }

    public T GetPlayerProperty<T>(PvpArenaUserPropertyType propType)
    {
        return (T) PlayerData.GetProperty<T>(propType);
    }

    public void SetOpponentProperty(PvpArenaUserInfo info)
    {
        OpponentData = info;
    }

    public void SetOpponentProperty(PvpArenaUserPropertyType key, object val)
    {
        OpponentData.SetProperty(key, val);

       /* AmtNetworkController.Instance.SendNetworkMessage<SendData>(new RequestDataPvpUpdatePlayerInfo(OpponentData), 
            CommandType.PvpUpdatePlayerInfo,withPreLoader:false);*/
    }

    private bool CheckMatchEnd()
    {
        var info = GetMatchData();

        if (info.EndTime < TimeManager.Instance.NowUnixSeconds
            || info.Players.Find(x => x.Walls >= 8) != null)
            return true;

        return false;
    }

    public void EndMatch(PvpArenaMatchInfo info)
    {
        if (info.Status == PvpArenaMatchStatusType.End)
            return;

        try
        {

            PvpHelper.EndPvpMatch(info, App.Instance.StaticData);

            ChangeGameState(PvpArenaMatchStatusType.End);

        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }


    private void Update()
    {
        if (_gameState == PvpArenaMatchStatusType.Started)
        {
            if (CheckMatchEnd())
            {
                EndMatch(GetMatchData());
            }
        }
    }
    public void SetOpponentProperty(Dictionary<PvpArenaUserPropertyType,object>  vals)
    {
        foreach (var val in vals)
        {
            OpponentData.SetProperty(val.Key, val.Value);
        }

        OnOpponentUpdated(OpponentData);


        /*    AmtNetworkController.Instance.SendNetworkMessage<SendData>(new RequestDataPvpUpdatePlayerInfo(OpponentData),
                CommandType.PvpUpdatePlayerInfo, withPreLoader: false);*/
    }

    public T GetOpponentProperty<T>(PvpArenaUserPropertyType propType)
    {
        return (T) OpponentData.GetProperty<T>(propType);
    }

    /*  public void ClearProperty()
      {
          PlayerData = new PvpArenaUserInfo();
          OpponentData = new PvpArenaUserInfo();

          SetOwnData();
      }*/
    public void SetPlayerProperty(Dictionary<PvpArenaUserPropertyType, object> vals)
    {
        var data = PlayerData.ToJson().FromJson<PvpArenaUserInfo>();

        foreach (var val in vals)
        {
            data.SetProperty(val.Key, val.Value);
        }

        SetOwnData(data);
      /*  AmtNetworkController.Instance.SendNetworkMessage<SendData>(new RequestDataPvpUpdatePlayerInfo(data),
            CommandType.PvpUpdatePlayerInfo, withPreLoader: false);*/
    }

    public void SetPlayerProperty(PvpArenaUserPropertyType propType, object ob)
  {

      PlayerData.SetProperty(propType, ob);

      /*  AmtNetworkController.Instance.SendNetworkMessage<SendData>(new RequestDataPvpUpdatePlayerInfo(data),
            CommandType.PvpUpdatePlayerInfo, withPreLoader: false);*/

  }

    public void SetOwnData(PvpArenaUserInfo info)
    {
        if (info != null)
        {
            PlayerData = info;

            var matchData = GetMatchData();
            var p = matchData.Players.Find(x => x.Id == PlayerData.Id);
            matchData.Players.Remove(p);
            matchData.AddPlayer(PlayerData);
            SaveMatchData(matchData);
        }
    }

    public void SetOwnData()
    {
       /* Debug.LogError("Set own data");
        SetPlayerProperty(PvpArenaUserPropertyType.Name, BlackTemple.EpicMine.Core.Player.Nickname);
        SetPlayerProperty(PvpArenaUserPropertyType.Id, BlackTemple.EpicMine.Core.Player.Id);
        SetPlayerProperty(PvpArenaUserPropertyType.Pickaxe,
            App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Id);
        SetPlayerProperty(PvpArenaUserPropertyType.Torch,
            App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.Id);

        var donateRang = 0;
        var donatePickCount =
            App.Instance.Player.Blacksmith.Pickaxes.FindAll(x => x.StaticPickaxe.Type == PickaxeType.Donate);
        if (donatePickCount.Count > 0)
        {
            for (var i = 0; i < donatePickCount.Count; i++)
            {
                if (App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Id ==
                    donatePickCount[i].StaticPickaxe.Id)
                {
                    donateRang = i;
                    break;
                }
            }
        }

        SetPlayerProperty(PvpArenaUserPropertyType.TorchDonateRang, donateRang);
        SetPlayerProperty(PvpArenaUserPropertyType.Torch,
            App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.Id);
        SetPlayerProperty(PvpArenaUserPropertyType.Rating, App.Instance.Player.Pvp.Rating);

        App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.PvpArenaPlayerData, PlayerData);

        */
    }

    public void SetOpponentData(PvpArenaUserInfo data)
    {
        if (data != null)
            OpponentData = data;

        var matchData = GetMatchData();
        var p = matchData.Players.Find(x => x.Id == OpponentData.Id);
        matchData.Players.Remove(p);
        matchData.AddPlayer(OpponentData);
        SaveMatchData(matchData);


        EventManager.Instance.Publish(new PvpArenaGetOpponentEvent(OpponentData));
    }

    public bool IsOpponentReady()
    {
        if (OpponentData != null && OpponentData.Ready)
            return true;

        return false;
    }

    private IEnumerator _startMatchCounter(float time)
    {
        yield return new WaitForSeconds(time);

    }

    public void EnableBot()
    {
        var bot = gameObject.GetComponent<PvpArenaBot>();
        if (bot != null)
            Destroy(bot);

        bot = gameObject.AddComponent<PvpArenaBot>();
        bot.Initialize(this, GetMatchData().Players.Find(x => x.IsBot));
    }

    public void OnConnected()
    {

    }

    public void OnDisconnected()
    {

        if (_botEnableCoroutine != null)
            StopCoroutine(_botEnableCoroutine);

        if (_matchStartCoroutine != null)
            StopCoroutine(_matchStartCoroutine);

        if (_connectCoroutine != null)
            StopCoroutine(_connectCoroutine);

        if (EventManager.Instance != null)
        {
            EventManager.Instance.Publish(new PvpArenaOnDisconnectedEvent());
        }

    }

    public void UserJoin(PvpArenaUserInfo info)
    {
        Debug.Log($"OnPlayerJoinRoom, nickname: {info.Name}");

        OnOpponentUpdated(info);
    }

    public void OnPlayerUpdate(PvpArenaUserInfo info)
    {
        SetOwnData(info); }

    public void OnOpponentUpdated(PvpArenaUserInfo info)
    {
        SetOpponentData(info);

        EventManager.Instance.Publish(new PvpArenaOpponentSectionPassedEvent(info.Walls));
    }

    public void UserLeft(PvpArenaUserInfo info)
    {
        
        Debug.Log($"OnPlayerLeftRoom, nickname: {info.Name}");

        EventManager.Instance.Publish(new PvpArenaPlayerLeaveGameEvent(info.Name));

        ClearMatchStorage();
        SceneManager.Instance.LoadScene(ScenesNames.PvpArena);

        return;
       /* if (_botEnableCoroutine != null)
            StopCoroutine(_botEnableCoroutine);

        if (_matchStartCoroutine != null)
            StopCoroutine(_matchStartCoroutine);

        if (_gameState == PvpArenaMatchStatusType.Lobby)
        {
            var matchData = GetMatchData();

            if (matchData!= null && matchData.Type != PvpArenaMatchType.Duel)
            {
                _botEnableCoroutine = StartCoroutine(EnableBot());
                return;
            }
            else
            {
                ClearMatchStorage();
                SceneManager.Instance.LoadScene(ScenesNames.PvpArena);
            }

        }

        OpponentData = new PvpArenaUserInfo();*/
    }

    public void OnMatchDataUpdate(PvpArenaMatchInfo data)
    {
      /*  Debug.Log("I");
        ChangeGameState(data.Status);

        var player = data.GetPlayer(App.Instance.Player.Id);

        OnPlayerUpdate(player);

        if (_botEnableCoroutine != null)
            StopCoroutine(_botEnableCoroutine);

        _botEnableCoroutine = StartCoroutine(_enableBot());

        var otherPlayer = data.Players.Find(x => x.Id != PlayerData.Id);

        if (otherPlayer != null && (OpponentData == null || string.IsNullOrEmpty(OpponentData.Id) || OpponentData.Id != otherPlayer.Id))
        {
            UserJoin(otherPlayer);
        }

        else if(otherPlayer == null && (OpponentData != null && !string.IsNullOrEmpty(OpponentData.Id)))
        {
            UserLeft(OpponentData);
        }*/
    }

    public void Leave(Action onLeave)
    {
        var data = GetMatchData();

        if (data != null)
        {
            var pl = data.Players.Find(x => !x.IsBot);
            if (pl != null)
            {
                pl.Leaved = true;
            }

            SaveMatchData(data);

            EndMatch(data);

        }

        onLeave?.Invoke();

        DestroyMatchData();

        EventManager.Instance.Publish(new PvpArenaOnClickLeaveRoomEvent());
    }

    public PvpArenaMatchInfo CreatePvpArena(int arena)
    {
        var staticData = App.Instance.StaticData;
        var player = App.Instance.Player;

        var ratingMax = player.Pvp.Rating + 200;
        var ratingMin = player.Pvp.Rating <= 200
            ? 0
            : player.Pvp.Rating - 200;

        var leagueSettingsNumber = PvpHelper.GetSuiteSettingsNumber();

        var filter = new PvpArenaMatchFilter
        {
            Prestige = player.Prestige,
            MinRating = ratingMin,
            MaxRating = ratingMax,
            LeagueSettingsNumber = leagueSettingsNumber,
            MaxUsers = 2
        };


        var tier = player.Dungeon.LastOpenedTier?.Number ?? 0;
        var walls = PvpHelper.GetWallsHealths(staticData, tier, leagueSettingsNumber, 8);

        var data = new PvpArenaMatchInfo(filter)
        {
            MaxPlayers = 2,
            Players = new List<PvpArenaUserInfo>(),
            Walls = walls,
            Type = PvpArenaMatchType.RandomMatch,
            Arena = arena,
            TimeOutTime = TimeManager.Instance.NowUnixSeconds + 60 * 60 * 24,
            EndTime = TimeManager.Instance.NowUnixSeconds + 183,
            MatchTime = 180
        };


        SaveMatchData(data);

        var opponent = SetBot(data, player);

        SetOwnData(PlayerData);

        SetOpponentData(opponent);
 

        SaveMatchData(data);

        if (OpponentData == null)
        {
            Debug.LogError("no user");
            return null;
        }

        return data;
    }

    public PvpArenaUserInfo SetBot(PvpArenaMatchInfo arena, Player player)
    {
        if (arena != null)
        {
           
                var botData = PvpHelper.GetPvpBotAccordingLeague(player, App.Instance.StaticData);
                if (botData == null)
                {
                    Debug.Log("cant find bot for player ");
                    return null;
                }

                var user = App.Instance.Controllers.RatingsController.GetPlayer(botData.Id);
                var rating = App.Instance.Controllers.RatingsController.GetPlayerPvpRating(user);

                if (user == null || rating == null)
                {
                    Debug.LogError("cant find rating bot for player ");
                    return null;
                }

                var botUser = new PvpArenaUserInfo
                {
                    Id = botData.Id,
                    Rating = rating.Rating,
                    IsBot = true,
                    Localate = user.Location,
                    Name = user.Name,
                    Pickaxe = PvpHelper.GetBotRandomPickAxe(player, App.Instance.StaticData, botData)?.Id,
                    Torch = PvpHelper.GetRandomTorch(player, App.Instance.StaticData, botData)?.Id,
                };


                arena.AddPlayer(botUser);

                return botUser;
        }
        else
        {
            Debug.LogError("Arena not exist");
            return null;
        }
    }

    public void JoinCreateRandom(int? arenaNumb = 0, Action onComplete = null)
    {
        if (_searchingMatch != null)
        {
            StopCoroutine(_searchingMatch);
        }

        ChangeGameState(PvpArenaMatchStatusType.Waiting);

        _searchingMatch = StartCoroutine(_searchingCoroutine(() =>
        {
            var arenaNumber = arenaNumb ?? PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating);
            Debug.Log("Join/created random");
            var data = CreatePvpArena(arenaNumber);

            SaveMatchData(data);

            if (_botEnableCoroutine != null)
            {
                StopCoroutine(_botEnableCoroutine);
            }

            EnableBot();

            onComplete?.Invoke();
        }));
    }

    private Coroutine _searchingMatch;

    private IEnumerator _searchingCoroutine(Action onEnd)
    {
        yield return new WaitForSeconds(Random.Range(2, 5));
        
        if (Application.internetReachability == NetworkReachability.NotReachable)
            yield break;

        onEnd?.Invoke();

        yield return null;

        ChangeGameState(PvpArenaMatchStatusType.Started);

    }
}