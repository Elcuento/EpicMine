using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Player = BlackTemple.EpicMine.Core.Player;
using SpriteHelper = BlackTemple.EpicMine.SpriteHelper;

public class WindowPvpArenaVersusScreen : WindowBase
{
    [SerializeField] private Image _backGround;
    [SerializeField] private GameObject _backGroundSpark;
    [SerializeField] private GameObject _closeButton;

    [Header("Left Player")]
    [SerializeField] private TextMeshProUGUI _leftPlayerName;
    [SerializeField] private TextMeshProUGUI _leftPlayerRating;

    [SerializeField] private Image _leftPlayerPickaxeBorder;
    [SerializeField] private Image _leftPlayerPickaxeIcon;
    [SerializeField] private Image _leftPlayerTorchBorder;
    [SerializeField] private Image _leftPlayerTorchIcon;


    [Header("Right Player")]
    [SerializeField] private TextMeshProUGUI _rightPlayerName;
    [SerializeField] private TextMeshProUGUI _rightPlayerRating;

    [SerializeField] private Image _rightPlayerPickaxeBorder;
    [SerializeField] private Image _rightPlayerPickaxeIcon;
    [SerializeField] private Image _rightPlayerTorchBorder;
    [SerializeField] private Image _rightPlayerTorchIcon;
    [SerializeField] private GameObject _rightPlayerLoader;

    public void Update()
    {
        var offset = new Vector2(Time.deltaTime/10, Time.deltaTime/10);
        _backGround.material.mainTextureOffset -= offset;
    }
    protected override void Awake()
    {
        base.Awake();

        ClearRightPlayer();

        _backGround.material = new Material(_backGround.material);

        EventManager.Instance.Subscribe<PvpArenaGetOpponentEvent>(OnGetOpponentData);
        EventManager.Instance.Subscribe<PvpArenaStartGameEvent>(OnGameStarted);
        EventManager.Instance.Subscribe<PvpArenaPlayerLeaveGameEvent>(OnPlayerLeaveGame);
        EventManager.Instance.Subscribe<PvpArenaFailedJoinedCustomRoomEvent>(OnFailedJoined);
        EventManager.Instance.Subscribe<PvpArenaGetCancelInviteEvent>(OnInviteCanceled);
        EventManager.Instance.Subscribe<PvpArenaGetDeniedInviteEvent>(OnInviteDenied);
        EventManager.Instance.Subscribe<PvpArenaOnDisconnectedEvent>(OnDisconnected);

        ShowSelf();

    }

    public void Start()
    {
        _backGroundSpark.transform.DOLocalRotate(new Vector3(0,0,360), 10, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(-1);
    }

    public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
    {
        base.OnShow(withPause, withCurrencies, withRating);

        CancelInvoke("TimeOut");

        var matchData = PvpArenaNetworkController.GetMatchData();
        if (matchData != null &&
            matchData.Type == PvpArenaMatchType.Duel)
        {
            Invoke("TimeOut", PvpLocalConfig.DuelTimeOut);
        }
    }

    public void TimeOut()
    {
        WindowManager.Instance.Show<WindowInformation>().Initialize("", "Waiting timeout",
            "window_place_for_chest_button", isNeedLocalizeDescription: false, isNeedLocalizeHeader: false);

        Close();
    }


    public void OnDestroy()
    {
        UnSubscribe();
    }

    public void UnSubscribe()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<PvpArenaGetOpponentEvent>(OnGetOpponentData);
            EventManager.Instance.Unsubscribe<PvpArenaStartGameEvent>(OnGameStarted);
            EventManager.Instance.Unsubscribe<PvpArenaPlayerLeaveGameEvent>(OnPlayerLeaveGame);
            EventManager.Instance.Unsubscribe<PvpArenaFailedJoinedCustomRoomEvent>(OnFailedJoined);
            EventManager.Instance.Unsubscribe<PvpArenaGetCancelInviteEvent>(OnInviteCanceled);
            EventManager.Instance.Unsubscribe<PvpArenaGetDeniedInviteEvent>(OnInviteDenied);
            EventManager.Instance.Unsubscribe<PvpArenaOnDisconnectedEvent>(OnDisconnected);
        }
    }

    public void ClearRightPlayer()
    {
        _closeButton.SetActive(true);

        _rightPlayerName.text = "";
        _rightPlayerRating.text = "";

        _rightPlayerPickaxeIcon.enabled = false;
        _rightPlayerTorchIcon.enabled = false;

        _rightPlayerPickaxeBorder.sprite = App.Instance.ReferencesTables.Sprites.Simple;
        _rightPlayerTorchBorder.sprite = App.Instance.ReferencesTables.Sprites.Simple;

        _rightPlayerLoader.gameObject.SetActive(true);
    }

    public void OnPlayerLeaveGame(PvpArenaPlayerLeaveGameEvent data)
    {
        ClearRightPlayer();

        var matchData = PvpArenaNetworkController.GetMatchData();
        if (matchData != null &&
            matchData.Type == PvpArenaMatchType.Duel)
        {
            Close();
        }
    }

    public void ShowSelf()
    {
        var matchData = PvpArenaNetworkController.GetMatchData();

        var myPlayer = matchData.Players.Find(x => x.Id == App.Instance.Player.Id);

        if (myPlayer == null)
        {
            PvpArenaNetworkController.DestroyMatchData();
            Close();
            SceneManager.Instance.LoadScene(ScenesNames.PvpArena);
            return;
        }

        _leftPlayerName.text = myPlayer.Name;
        _leftPlayerRating.text = myPlayer.Rating.ToString();

        _leftPlayerPickaxeIcon.sprite = SpriteHelper.GetPickaxeImage(myPlayer.Pickaxe);
        _leftPlayerPickaxeIcon.SetNativeSize();

        _leftPlayerTorchIcon.sprite = SpriteHelper.GetTorchImage(myPlayer.Torch);
        _leftPlayerTorchIcon.SetNativeSize();

        _leftPlayerPickaxeBorder.sprite = SpriteHelper.GetPickaxeRarityBackgroundLuxAndUsual(myPlayer.Pickaxe, myPlayer.DonateRang);
        _leftPlayerTorchBorder.sprite = SpriteHelper.GetTorchBackground(App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.Id);
    }

    public void OnGetOpponentData(PvpArenaGetOpponentEvent data)
    {
        CancelInvoke("TimeOut");

        StopAllCoroutines();

        _rightPlayerLoader.gameObject.SetActive(false);

        _rightPlayerPickaxeIcon.enabled = !string.IsNullOrEmpty(data.Data.Pickaxe);
        _rightPlayerTorchIcon.enabled = !string.IsNullOrEmpty(data.Data.Torch);

        _rightPlayerName.text = data.Data.Name.Contains(PvpLocalConfig.BotNamePrefix) ?
            data.Data.Name.Replace(PvpLocalConfig.BotNamePrefix, "") : data.Data.Name;

        _rightPlayerRating.text = data.Data.Rating.ToString();

        _rightPlayerPickaxeIcon.sprite = SpriteHelper.GetPickaxeImage(data.Data.Pickaxe);
        _rightPlayerPickaxeIcon.SetNativeSize();

        _rightPlayerTorchIcon.sprite = SpriteHelper.GetTorchImage(data.Data.Torch);
        _rightPlayerTorchIcon.SetNativeSize();

        _rightPlayerPickaxeBorder.sprite = SpriteHelper.GetPickaxeRarityBackgroundLuxAndUsual(data.Data.Pickaxe, data.Data.DonateRang);
        _rightPlayerTorchBorder.sprite = SpriteHelper.GetTorchBackground(data.Data.Torch);

        _closeButton.SetActive(false);

    }

    public void OnDisconnected(PvpArenaOnDisconnectedEvent eventData)
    {
        Close();
    }

    public void OnInviteDenied(PvpArenaGetDeniedInviteEvent eventData)
    {
        WindowManager.Instance.Show<WindowInformation>().Initialize("pvp_invite_denied", "pvp_invite_denied_disc", "window_place_for_chest_button");
        Close();
    }

    public void OnInviteCanceled(PvpArenaGetCancelInviteEvent eventData)
    {
        WindowManager.Instance.Show<WindowInformation>().Initialize("pvp_invite_denied", "pvp_invite_denied_disc", "window_place_for_chest_button");
        Close();
    }

    public void OnFailedJoined(PvpArenaFailedJoinedCustomRoomEvent eventData)
    {
       Close();
    }

    public void OnGameStarted(PvpArenaStartGameEvent data)
    {
        WindowManager.Instance.Close(this,true);
    }

    public override void Close()
    {
        var matchData = PvpArenaNetworkController.GetMatchData();

        if (matchData != null)
        {
            if (matchData.Type == PvpArenaMatchType.Duel && matchData.Players.Count > 0)
            {
                var otherPlayers = matchData.Players.FindAll(x => x.Id != App.Instance.Player.Id);
                foreach (var pvpArenaUserInfo in otherPlayers)
                {
                    App.Instance.Player.Pvp.SendDeniedInvite(pvpArenaUserInfo.Id);
                }
          
            }
        }

        UnSubscribe();
        CancelInvoke("TimeOut");

        PvpArenaNetworkController.I.Leave(() =>
        {
            SceneManager.Instance.LoadScene(ScenesNames.PvpArena);
        });
    }

    public void OnClickClose()
    {
        Close();
    }

}

