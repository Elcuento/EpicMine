using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Dto;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Tier = BlackTemple.EpicMine.Core.Tier;

public class AutoMinerSceneStatusInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tierName;
    [SerializeField] private TextMeshProUGUI _tierNumber;

    [Header("Level")]
    [SerializeField] private GameObject _levelContent;
    [SerializeField] private Image _levelFillBar;
    [SerializeField] private TextMeshProUGUI _levelTitle;

    [Header("Harvest")]
    [SerializeField] private Slider _harvestFillBar;
    [SerializeField] private TextMeshProUGUI _harvestTimeLeft;

    [Header("Status panel / Buttons")]
    [SerializeField] private GameObject _fullLabel;
    [SerializeField] private GameObject _progressLabel;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _startedButton;
    [SerializeField] private GameObject _lockedButton;

    [Space]
    [SerializeField] private List<AutoMinerSceneDropItem> _dropItems;

    [Header("Upgrades")]
    [SerializeField] private Sprite _canUpgradeIcon;
    [SerializeField] private Sprite _cantUpgradeIcon;

    [Header("Capacity")]
    [SerializeField] private TextMeshProUGUI _capacityTitle;
    [SerializeField] private TextMeshProUGUI _capacityCost;
    [SerializeField] private Button _capacityButton;
    [SerializeField] private ItemView _capacityItemIcon;
    [SerializeField] private Image _capacityUpgradeIcon;

    [Header("Speed")]
    [SerializeField] private TextMeshProUGUI _speedTitle;
    [SerializeField] private TextMeshProUGUI _speedCost;
    [SerializeField] private Button _speedButton;
    [SerializeField] private ItemView _speedItemIcon;
    [SerializeField] private Image _speedUpgradeIcon;

    private Tier _tier;

    private void Start()
    {
        EventManager.Instance.Subscribe<AutoMinerChangeEvent>(OnAutoMinerChange);
        EventManager.Instance.Subscribe<AutoMinerChangeCapacityLevelEvent>(OnAutoMinerCapacityChangeLevel);
        EventManager.Instance.Subscribe<AutoMinerChangeSpeedLevelEvent>(OnAutoMinerSpeedChangeLevel);
        EventManager.Instance.Subscribe<SecondsTickEvent>(OnTimeTick);
        EventManager.Instance.Subscribe<AutoMinerChangeMinerLevelEvent>(OnMinerChangeLevel);

        UpdateSpeedView();
        UpdateCapacityView();
        UpdateLevel();
    }



    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<AutoMinerChangeEvent>(OnAutoMinerChange);
        EventManager.Instance.Unsubscribe<AutoMinerChangeCapacityLevelEvent>(OnAutoMinerCapacityChangeLevel);
        EventManager.Instance.Unsubscribe<AutoMinerChangeSpeedLevelEvent>(OnAutoMinerSpeedChangeLevel);
        EventManager.Instance.Unsubscribe<AutoMinerChangeMinerLevelEvent>(OnMinerChangeLevel);
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTimeTick);
    }


    private void OnMinerChangeLevel(AutoMinerChangeMinerLevelEvent eventData)
    {
        UpdateLevel();
    }


    private void OnTimeTick(SecondsTickEvent eventData)
    {
        if (!App.Instance.Player.AutoMiner.Started || App.Instance.Player.AutoMiner.Tier != _tier.Number || App.Instance.Player.AutoMiner.IsFull)
            return;

        _harvestTimeLeft.text = $"{App.Instance.Player.AutoMiner.TimeLeft} {LocalizationHelper.GetLocale("seconds")}";
    }

    private void UpdateLevel()
    {
        _levelTitle.text = $"{ (AutoMinerHelper.GetLevelCurrentValueProgress() + 1) }/{ AutoMinerHelper.GetLevelNextValueProgress() }";
        _levelFillBar.fillAmount = AutoMinerHelper.GetLevelFillProgress();
    }

    public void Initialize(Tier tier)
    {
        _tier = tier;

        var mine = _tier.Mines.Count > 2 ? _tier.Mines[2] : _tier.Mines.First();
   
        var staticTier = StaticHelper.GetTier(_tier.Number);
        var staticMine = StaticHelper.GetMine(_tier.Number, mine.Number);

        var mineCommonSettings = StaticHelper.GetMineCommonSettings(_tier.Number, mine.Number);

        var tierName = LocalizationHelper.GetLocale("tier_" + _tier.Number);
        var tierNumber = $"{(_tier.Number + 1)} { LocalizationHelper.GetLocale("tier") }: ";

        _tierName.text = tierName;
        _tierNumber.text = tierNumber.ToLower();

        var firstItemChance = staticMine.Item1Percent ?? mineCommonSettings.Item1Percent;
        var secondItemChance = staticMine.Item2Percent ?? mineCommonSettings.Item2Percent;
        var thirdItemChance = staticMine.Item3Percent ?? mineCommonSettings.Item3Percent;

        // exclude chest percent
        var itemsChanceSum = firstItemChance + secondItemChance + thirdItemChance;
        firstItemChance = Mathf.RoundToInt(firstItemChance / itemsChanceSum * 100);
        secondItemChance = Mathf.RoundToInt(secondItemChance / itemsChanceSum * 100);
        thirdItemChance = Mathf.RoundToInt(thirdItemChance / itemsChanceSum * 100);

        _dropItems[0].Initialize(
            staticTier.WallItem1Id,
            firstItemChance,
            _tier.IsDropItemUnblocked(staticTier.WallItem1Id));

        _dropItems[1].Initialize(
            staticTier.WallItem2Id,
            secondItemChance,
            _tier.IsDropItemUnblocked(staticTier.WallItem2Id));

        _dropItems[2].Initialize(
            staticTier.WallItem3Id,
            thirdItemChance,
            _tier.IsDropItemUnblocked(staticTier.WallItem3Id));

        UpdateState();

    }


    private void OnAutoMinerSpeedChangeLevel(AutoMinerChangeSpeedLevelEvent data)
    {
        UpdateSpeedView();
        UpdateCapacityView();
        UpdateState();
    }

    private void OnAutoMinerCapacityChangeLevel(AutoMinerChangeCapacityLevelEvent data)
    {
        UpdateSpeedView();
        UpdateCapacityView();
        UpdateState();
    }

    private void OnAutoMinerChange(AutoMinerChangeEvent eventData)
    {
        UpdateState();
    }

    private void UpdateState()
    {
        _fullLabel.SetActive(false);
        _harvestFillBar.gameObject.SetActive(false);
        _harvestFillBar.DOKill();
        _startedButton.SetActive(false);
        _progressLabel.SetActive(false);
        _startButton.SetActive(false);
        _lockedButton.SetActive(false);
        _harvestTimeLeft.text = ". . .";

        if (!_tier.IsOpen)
        {
            _lockedButton.SetActive(true);
            return;
        }

        if (!App.Instance.Player.AutoMiner.Started)
        {
            _startButton.SetActive(true);
            return;
        }

      
        if (App.Instance.Player.AutoMiner.IsFull)
        {
            _fullLabel.SetActive(true);
            _harvestFillBar.gameObject.SetActive(true);
            _startedButton.SetActive(false);
        }
        else
        {

            if (App.Instance.Player.AutoMiner.Tier == _tier.Number)
            {
                _harvestFillBar.gameObject.SetActive(true);
                _progressLabel.SetActive(true);

                var value = (App.Instance.Player.AutoMiner.Cost - App.Instance.Player.AutoMiner.TimeLeft) /
                            (float)App.Instance.Player.AutoMiner.Cost;

                _harvestFillBar.value = value;
                _harvestFillBar.DOValue(1, App.Instance.Player.AutoMiner.TimeLeft)
                    .SetEase(Ease.Linear);
            }
            else
            {
                _harvestFillBar.gameObject.SetActive(true);
                _harvestFillBar.value = 0;
                _startButton.SetActive(true);
               
            }
        }

        OnTimeTick(new SecondsTickEvent());

    }

    public void ClickStart()
    {
        App.Instance.Player.AutoMiner.StartAutoMiner(_tier.Number);
    }

    public void ClickUpgradeSpeedLevel()
    {
        App.Instance.Player.AutoMiner.SpeedLevel.Up();
    }

    public void ClickUpgradeCapacityLevel()
    {
        App.Instance.Player.AutoMiner.CapacityLevel.Up();
    }

    private void UpdateSpeedView()
    {
        UpdateLevel();

        var level = App.Instance.Player.AutoMiner.SpeedLevel;
        var isLastLevel = level.NextStaticLevel == null;

        _speedCost.text = $"{level.Number:F2}";
        _speedUpgradeIcon.sprite = _cantUpgradeIcon;


        if (isLastLevel)
        {
            _speedButton.gameObject.SetActive(false);
            _speedItemIcon.gameObject.SetActive(false);
            _levelContent.gameObject.SetActive(false);
            _speedTitle.text = $"{ level.StaticLevel.Amount }";
            return;
        }

        if (level.NextStaticLevel.CurrencyCost.Count > 0)
        {
            var currency = level.NextStaticLevel.CurrencyCost.FirstOrDefault();

            _speedItemIcon.Initialize(SpriteHelper.GetCurrencyIcon(currency.Key), string.Empty);

            var dtoCurrency = new BlackTemple.EpicMine.Dto.Currency(currency.Key, currency.Value);
            var hasCurrency = App.Instance.Player.Wallet.Has(dtoCurrency);

            var color =
                (hasCurrency ? Color.white : Color.red).CovertToHex();

            _speedCost.text = $"<color=#{color}>{ (currency.Value) }</color>";


            _speedUpgradeIcon.sprite = hasCurrency
                ? _canUpgradeIcon
                : _cantUpgradeIcon;

        }
        else if (level.NextStaticLevel.ItemsCost.Count  > 0)
        {
            var item = level.NextStaticLevel.ItemsCost.FirstOrDefault();

            _speedItemIcon.Initialize(item.Key, string.Empty);
            _speedItemIcon.EnableRaycast(true);

            var dtoItem = new Item(item.Key, (int)item.Value);
            var hasItem = App.Instance.Player.Inventory.Has(dtoItem);

            var color =
                (hasItem ? Color.white : Color.red).CovertToHex();

            _speedCost.text = $"<color=#{ color }>{ item.Value }</color>";

            //    _speedButton.image.sprite = hasItem
            //        ? App.Instance.ReferencesTables.Sprites.ButtonGreen
            //       : App.Instance.ReferencesTables.Sprites.ButtonGrey;

            _speedUpgradeIcon.sprite = hasItem
                ? _canUpgradeIcon
                : _cantUpgradeIcon;
        }
        else
        {

            _speedButton.gameObject.SetActive(false);
            _speedItemIcon.gameObject.SetActive(false);
            _levelContent.gameObject.SetActive(false);
            _speedTitle.text = $"{ level.StaticLevel.Amount }";
            return;
        }

        var diff = level.NextStaticLevel.Amount - level.StaticLevel.Amount;
        _speedTitle.text = $"{ level.StaticLevel.Amount } <color=#00E100><size=35>(+{ diff })</size></color>";
    }

    private void UpdateCapacityView()
    {
        var level = App.Instance.Player.AutoMiner.CapacityLevel;
        var isLastLevel = level.NextStaticLevel == null;

        _capacityCost.text = $"{level.Number:F2}";
        _capacityUpgradeIcon.sprite = _cantUpgradeIcon;

        if (isLastLevel)
        {
            _capacityButton.gameObject.SetActive(false);
            _capacityItemIcon.gameObject.SetActive(false);
            _capacityTitle.text = $"{ level.StaticLevel.Capacity }";
            return;
        }

        if (level.NextStaticLevel.CurrencyCost.Count > 0)
        {
            var currency = level.NextStaticLevel.CurrencyCost.FirstOrDefault();

            _capacityItemIcon.Initialize(SpriteHelper.GetCurrencyIcon(currency.Key), string.Empty);

            var dtoCurrency = new BlackTemple.EpicMine.Dto.Currency(currency.Key, currency.Value);
            var hasCurrency = App.Instance.Player.Wallet.Has(dtoCurrency);

            var color =
                (hasCurrency ? Color.white : Color.red).CovertToHex();

            _capacityCost.text = $"<color=#{ color }>{ currency.Value }</color>";
            //  _capacityButton.image.sprite = hasCurrency
            //      ? App.Instance.ReferencesTables.Sprites.ButtonGreen
            //      : App.Instance.ReferencesTables.Sprites.ButtonGrey;

            _capacityUpgradeIcon.sprite = hasCurrency
                ? _canUpgradeIcon
                : _cantUpgradeIcon;

        }
        else if (level.NextStaticLevel.ItemsCost.Count > 0)
        {
            var item = level.NextStaticLevel.ItemsCost.FirstOrDefault();

            _capacityItemIcon.Initialize(item.Key, string.Empty);
            _capacityItemIcon.EnableRaycast(true);

            var dtoItem = new Item(item.Key, (int)item.Value);
            var hasItem = App.Instance.Player.Inventory.Has(dtoItem);

            var color =
                (hasItem ? Color.white : Color.red).CovertToHex();

            _capacityCost.text = $"<color=#{ color }>{ item.Value }</color>";

            //  _capacityButton.image.sprite = hasItem
            //      ? App.Instance.ReferencesTables.Sprites.ButtonGreen
            //     : App.Instance.ReferencesTables.Sprites.ButtonGrey;

            _capacityUpgradeIcon.sprite = hasItem
                   ? _canUpgradeIcon
                   : _cantUpgradeIcon;
        }
        else
        {
            _capacityButton.gameObject.SetActive(false);
            _capacityItemIcon.gameObject.SetActive(false);
            _capacityTitle.text = $"{ level.StaticLevel.Capacity }";
        }

        var diff = level.NextStaticLevel.Capacity - level.StaticLevel.Capacity;
        _capacityTitle.text = $"{ level.StaticLevel.Capacity } <color=#00E100><size=35>(+{ diff })</size></color>";
    }

}
