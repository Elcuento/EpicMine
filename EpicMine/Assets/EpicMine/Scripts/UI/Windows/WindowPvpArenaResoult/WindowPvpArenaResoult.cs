using System.Collections;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowPvpArenaResoult : WindowBase {

    [SerializeField] private TextMeshProUGUI _resoultLabel;
    [SerializeField] private TextMeshProUGUI _leagueNumber;
    [SerializeField] private TextMeshProUGUI _leagueName;

    [SerializeField] private Button _nextCloseButton;
    [SerializeField] private TextMeshProUGUI _nextCloseButtonText;
    [SerializeField] private GameObject _nextCloseButtonLoadingArrow;

    [Header("Left Player")]

    [SerializeField] private TextMeshProUGUI _leftPlayerName;
    [SerializeField] private TextMeshProUGUI _leftPlayerRating;
    [SerializeField] private TextMeshProUGUI _leftPlayerDamageDone;
    [SerializeField] private TextMeshProUGUI _leftPlayerWallsPassed;

    [SerializeField] private Image _leftPlayerPickaxeBorder;
    [SerializeField] private Image _leftPlayerPickaxeIcon;
    [SerializeField] private Image _leftPlayerTorchBorder;
    [SerializeField] private Image _leftPlayerTorchIcon;
    [SerializeField] private Image _leftPlayerChest;

    [SerializeField] private GameObject _leftPlayerWinnerStuff;

    [Header("Typs")]

    [SerializeField] private GameObject _leftPlayerChestTyp;


    [Header("Right Player")]
    [SerializeField] private TextMeshProUGUI _rightPlayerName;
    [SerializeField] private TextMeshProUGUI _rightPlayerRating;
    [SerializeField] private TextMeshProUGUI _rightPlayerDamageDone;
    [SerializeField] private TextMeshProUGUI _rightPlayerWallsPassed;

    [SerializeField] private Image _rightPlayerPickaxeBorder;
    [SerializeField] private Image _rightPlayerPickaxeIcon;
    [SerializeField] private Image _rightPlayerTorchBorder;
    [SerializeField] private Image _rightPlayerTorchIcon;
    [SerializeField] private Image _rightPlayerChest;

    [SerializeField] private GameObject _rightPlayerWinnerStuff;

    [Header("DueldTyp")]
    [SerializeField] private GameObject _duelTypPanel;

    [Header("Chests")]
    [SerializeField] private GameObject _chestPanel;
    [SerializeField] private TextMeshProUGUI _chests;
    [SerializeField] private Image _chestsFill;
    [SerializeField] private GameObject _chestCollectPanel;
    [SerializeField] private GameObject _chestGetPanel;


    private PvpChestType _lastAddedChestType;

    private PvpArenaMatchType _type;

    private PvpArenaNetworkController _controller;

    public void Start()
    {

        EventManager.Instance.Subscribe<PvpArenaEndGameResoultEvent>(OnGetEndGameResoult);

        SetStartButtonDefault();
    }

    public void OnDestroy()
    {
        if (EventManager.Instance != null) 
        {
            EventManager.Instance.Unsubscribe<PvpArenaEndGameResoultEvent>(OnGetEndGameResoult);
        }
    }


    public void Initialzie(PvpArenaNetworkController control, PvpArenaGameResoultType resoult, PvpArenaUserInfo self, PvpArenaUserInfo opponent, PvpArenaMatchType matchType)
    {
        _type = matchType;
        _controller = control;


        _nextCloseButtonText.text = matchType == PvpArenaMatchType.RandomMatch
            ? LocalizationHelper.GetLocale("window_pvp_resoult_nextfight")
            : LocalizationHelper.GetLocale("window_pvp_resoult_finish");

        _duelTypPanel.SetActive(matchType == PvpArenaMatchType.Duel);
        _chestPanel.SetActive(matchType != PvpArenaMatchType.Duel);

        if (resoult == PvpArenaGameResoultType.Win && matchType == PvpArenaMatchType.RandomMatch)
        {
            _lastAddedChestType = (PvpChestType)self.Chest;

            _leftPlayerChest.sprite = SpriteHelper.GetIcon(_lastAddedChestType == PvpChestType.Simple
                ? PvpLocalConfig.PvpSimpleChestItemId
                : PvpLocalConfig.PvpRoyalChestItemId);

            var id = _lastAddedChestType == PvpChestType.Simple ? PvpLocalConfig.PvpSimpleChestItemId :
                _lastAddedChestType == PvpChestType.Royal ? PvpLocalConfig.PvpRoyalChestItemId :
                PvpLocalConfig.PvpWinnerChestItemId;

            App.Instance.Player.Inventory.Add(new CommonDLL.Dto.Item(id, 1),IncomeSourceType.FromPvp);

        _leftPlayerChestTyp.SetActive(!PlayerPrefsHelper.LoadDefault(PlayerPrefsType.WindowPvpResultTyp1, false));
            _leftPlayerChest.gameObject.SetActive(true);
        }
        else _leftPlayerChest.gameObject.SetActive(false);

        if (resoult == PvpArenaGameResoultType.Loose && matchType == PvpArenaMatchType.RandomMatch)
        {
            var chestType = (PvpChestType)opponent.Chest;
            _rightPlayerChest.sprite = SpriteHelper.GetIcon(chestType == PvpChestType.Simple
                ? PvpLocalConfig.PvpSimpleChestItemId
                : PvpLocalConfig.PvpRoyalChestItemId);
            _rightPlayerChest.gameObject.SetActive(true);
        }
        else _rightPlayerChest.gameObject.SetActive(false);

        _leftPlayerWinnerStuff.SetActive(resoult == PvpArenaGameResoultType.Win);
        _rightPlayerWinnerStuff.SetActive(resoult == PvpArenaGameResoultType.Loose);

        UpdateSelf(self);

        _resoultLabel.text = LocalizationHelper.GetLocale($"pvp_game_resoult_{resoult}");

        _rightPlayerName.text = opponent.Name.Contains(PvpLocalConfig.BotNamePrefix) ? 
            opponent.Name.Replace(PvpLocalConfig.BotNamePrefix,"") : opponent.Name;

        _rightPlayerRating.text = opponent.Rating.ToString();

        _rightPlayerWallsPassed.text = (opponent.Walls > PvpLocalConfig.DefaultPvpMineSectionCount ? 
            PvpLocalConfig.DefaultPvpMineSectionCount : opponent.Walls).ToString();
        _rightPlayerDamageDone.text = opponent.Damage.ToString();

        _rightPlayerPickaxeIcon.sprite = SpriteHelper.GetPickaxeImage(opponent.Pickaxe);
        _rightPlayerPickaxeIcon.SetNativeSize();

        _rightPlayerTorchIcon.sprite = SpriteHelper.GetTorchImage(opponent.Torch);
        _rightPlayerTorchBorder.sprite = SpriteHelper.GetTorchBackground(opponent.Torch);
        _rightPlayerTorchIcon.SetNativeSize();

        _rightPlayerPickaxeBorder.sprite =
           SpriteHelper.GetPickaxeRarityBackgroundLuxAndUsual(opponent.Pickaxe, opponent.DonateRang);

   //

    }

    public void OnGetEndGameResoult(PvpArenaEndGameResoultEvent eventData)
    {

        OnPvpOpponentDataUpdate(eventData);
        OnSelfDataUpdate(eventData);

    }

    public void OnPvpOpponentDataUpdate(PvpArenaEndGameResoultEvent evenData)
    {
        if (evenData.OpponentChangeRating == 0) return;

        var isIncrease = evenData.OpponentChangeRating > 0;
        var color = isIncrease ? "#7CFC00" : "#FF0000";
        var symbol = isIncrease ? "+" : "";

        _rightPlayerRating.text = $"{evenData.OpponentRating} <color={color}> {symbol} {evenData.OpponentChangeRating}</color>";
    }

    public void OnSelfDataUpdate(PvpArenaEndGameResoultEvent ev)
    {
        if (ev.PlayerChangeRating != 0)
        {
            var isIncrease = ev.PlayerChangeRating > 0;
            var color = isIncrease ? "#7CFC00" : "#FF0000";
            var symbol = isIncrease ? "+" : "";

            _leftPlayerRating.text = $"{ev.PlayerRating} <color={color}> {symbol} {ev.PlayerChangeRating}</color>";
        }

        UpdateChests();

    }


    public void UpdateSelf(PvpArenaUserInfo player)
    {
        _leftPlayerWallsPassed.text = (player.Walls > PvpLocalConfig.DefaultPvpMineSectionCount ? PvpLocalConfig.DefaultPvpMineSectionCount : player.Walls).ToString();
        _leftPlayerDamageDone.text = player.Damage.ToString();

        _leftPlayerName.text = player.Name;
        _leftPlayerRating.text = player.Rating.ToString();

        _leftPlayerPickaxeIcon.sprite = SpriteHelper.GetPickaxeImage(player.Pickaxe);
        _leftPlayerPickaxeIcon.SetNativeSize();

        _leftPlayerTorchIcon.sprite = SpriteHelper.GetTorchImage(player.Torch);
        _leftPlayerTorchIcon.SetNativeSize();

        _leftPlayerPickaxeBorder.sprite
            = SpriteHelper.GetPickaxeRarityBackgroundLuxAndUsual(player.Pickaxe, player.DonateRang);
         _leftPlayerTorchBorder.sprite = SpriteHelper.GetTorchBackground(App.Instance.Player.TorchesMerchant.SelectedTorch);

        var league = PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating);
        _leagueNumber.text = LocalizationHelper.GetLocale("league") + " " + (league + 1);
        _leagueName.text = LocalizationHelper.GetLocale("league_" + (league + 1));

        UpdateChests();
    }

    public void OnClickGetChest()
    {
        var selectedTier = App.Instance.Player.Dungeon.LastOpenedTier.Number;

        WindowManager.Instance.Show<WindowOpenPvpChest>(withPause: false, withCurrencies: true)
            .Initialize(PvpChestType.Winner, selectedTier, false, OnRewardRecived);
    }

    public void OnRewardRecived()
    {
        _chestCollectPanel.SetActive(true);
        _chestGetPanel.SetActive(false);
        _chests.text = $"0/{PvpLocalConfig.PvpWinChestRequire}";
        _chestsFill.fillAmount = 0;
    }

    public void UpdateChests()
    {
        _chestCollectPanel.SetActive(App.Instance.Player.Pvp.Chests != PvpLocalConfig.PvpWinChestRequire);
        _chestGetPanel.SetActive(App.Instance.Player.Pvp.Chests == PvpLocalConfig.PvpWinChestRequire);

        _chestsFill.fillAmount = App.Instance.Player.Pvp.Chests / (float)PvpLocalConfig.PvpWinChestRequire;
        _chests.text = $"{App.Instance.Player.Pvp.Chests}/{PvpLocalConfig.PvpWinChestRequire}";
    }

    public void OnClickNextFight()
    {
        if (_type == PvpArenaMatchType.RandomMatch)
            StartNextMatch();
        else SceneManager.Instance.LoadScene(ScenesNames.PvpArena);

    }

    public void StartNextMatch()
    {
        PvpArenaNetworkController.DestroyMatchData();

        SceneManager.Instance.LoadScene(ScenesNames.PvpArena);

        PvpArenaNetworkController.I.JoinCreateRandom(null,() =>
        {

        });
    }

    public void OnClickClose()
    {
        PvpArenaNetworkController.I.Leave(() =>
        {
            SceneManager.Instance.LoadScene(ScenesNames.PvpArena);
        });
    }

    public void OnClickOpenTaskAddedChest()
    {
        var selectedTier = App.Instance.Player.Dungeon.LastOpenedTier.Number;

        _leftPlayerChest.gameObject.SetActive(false);

        WindowManager.Instance.Show<WindowOpenPvpChest>(withPause: false, withCurrencies: true)
            .Initialize(_lastAddedChestType, selectedTier, true);

    }

    public void OnClickTyp1()
    {
        PlayerPrefsHelper.Save(PlayerPrefsType.WindowPvpResultTyp1, true);
        _leftPlayerChestTyp.SetActive(false);
    }

    private void SetStartButtonLoading()
    {
        _nextCloseButtonText.gameObject.SetActive(false);
        _nextCloseButtonLoadingArrow.gameObject.SetActive(true);
        _nextCloseButton.interactable = false;

    }

    private void SetStartButtonDefault()
    {
        _nextCloseButton.interactable = true;
        _nextCloseButtonText.gameObject.SetActive(true);
        _nextCloseButtonLoadingArrow.gameObject.SetActive(false);

    }

    public IEnumerator ClickNextCoroutine()
    {
        SetStartButtonLoading();

        yield return new WaitForSeconds(1);

        while (_controller.IsConnected)
        {
            yield return new WaitForEndOfFrame();
        }

        StartNextMatch();
    }
}
