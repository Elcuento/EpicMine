using System;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;
using Transform = UnityEngine.Transform;

public class WindowGetAutoMinerEarning : WindowBase
{
    private Action _onClose;

    [SerializeField] private GameObject _lights;

    [Space]
    [SerializeField] private GameObject _doubleBonusCrystalButton;
    [SerializeField] private TextMeshProUGUI _doubleBonusCrystalButtonLabel;
    [SerializeField] private GameObject _doubleBonusAdButton;
    [SerializeField] private GameObject _takeDoubleBonusButton;
    [SerializeField] private GameObject _takeBonusButton;

    [SerializeField] private TextMeshProUGUI _getItemText;

    [Space]
    [SerializeField] private TextMeshProUGUI _headerText;

    [Space]
    [SerializeField] private ItemView _dropItemPrefab;
    [SerializeField] private Transform _dropItemsContainer;

    private bool _isDoubled;
    private bool _isGetReward;
    private Pack _dropPack;


    public void Initialize(Dictionary<string, int> items, Action onClose = null)
    {
        Clear();

        _onClose = onClose;

        _headerText.text = LocalizationHelper.GetLocale("window_get_auto_miner_earning");
        _doubleBonusCrystalButtonLabel.text = App.Instance.StaticData.Configs.Pvp.ChestDoubleBonusCrystalsCost.ToString();

        App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnRewardShowed;

        _dropPack = new Pack(items);

        foreach (var item in _dropPack.Items)
        {
            if (item.Amount <= 0) continue;

            var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
            itemView.Initialize(item);
        }
        _lights.SetActive(true);


        if (_dropPack.TotalItemsCount >= 10)
        {
            _takeBonusButton.gameObject.SetActive(true);
        }
        else
        {
            _doubleBonusAdButton.gameObject.SetActive(false);
            _doubleBonusCrystalButton.gameObject.SetActive(false);
            _takeBonusButton.gameObject.SetActive(false);
            _takeDoubleBonusButton.gameObject.SetActive(true);
            _getItemText.text = LocalizationHelper.GetLocale("window_workshop_collect_button");
        }

    }

    private void Clear()
    {
        _onClose = null;
        _isDoubled = false;
        _isGetReward = false;
        _dropPack = null;
        _getItemText.text = LocalizationHelper.GetLocale("window_workshop_collect_button");

        _lights.SetActive(false);

        _doubleBonusAdButton.SetActive(true);
        _doubleBonusCrystalButton.SetActive(true);
        _takeBonusButton.gameObject.SetActive(false);
        _takeDoubleBonusButton.gameObject.SetActive(false);

        _dropItemsContainer.ClearChildObjects();

        if (App.Instance != null)
            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnRewardShowed;
    }


    public void OnClickGetDoubleBonusByCrystal()
    {
        var currency = new Currency(CurrencyType.Crystals, App.Instance.StaticData.Configs.Pvp.ChestDoubleBonusCrystalsCost);
        if (!App.Instance.Player.Wallet.Has(currency))
        {
            WindowManager.Instance.Show<WindowShop>()
                .OpenCrystals();
            return;
        }

        App.Instance.Player.Wallet.Remove(currency);

        _doubleBonusAdButton.gameObject.SetActive(false);
        _doubleBonusCrystalButton.gameObject.SetActive(false);
        _takeDoubleBonusButton.gameObject.SetActive(true);
        _takeBonusButton.gameObject.SetActive(false);

        var parameters = new CustomEventParameters
        {
            Int = new Dictionary<string, int>
            {
                {"spent_crystals", (int)currency.Amount },
            }
        };
        App.Instance.Services.AnalyticsService.CustomEvent("complete_autominer_double", parameters);

        Double();

    }

    public void OnClickGetDoubleBonusByAd()
    {

        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
        App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.MultiplyPvpChestReward);
    }

    public override void OnClose()
    {
        base.OnClose();
        _onClose?.Invoke();
        Clear();
    }

    public void OnClickGetReward()
    {
        if (_isGetReward)
            return;

        App.Instance.Player.AutoMiner.Collect((amount) =>
        {
            if (amount != _dropPack.TotalItemsCount)
            {
                Debug.Log(_dropPack.TotalItemsCount + " to match, fix items to " + amount);
                var correctItems = new List<Item>();
                var itemCoefficient = amount / (float)_dropPack.TotalItemsCount;

                foreach (var item in _dropPack.Items)
                {
                    var quantity = (int)(item.Amount * itemCoefficient);
                    correctItems.Add(new Item(item.Id, quantity));
                }

                _dropPack = new Pack(correctItems);
            }
            App.Instance.Player.Inventory.Add(_dropPack, IncomeSourceType.FromPvp);
            _isGetReward = true;
            Close();
        }, _isDoubled);
    }

    public void OnRewardShowed(AdSource adSource, bool isShowed, string rewardId, int rewardAmount)
    {
        if (!isShowed)
            return;

        if (adSource != AdSource.MultiplyPvpChestReward)
            return;

        if (_isDoubled)
            return;

        _doubleBonusAdButton.gameObject.SetActive(false);
        _doubleBonusCrystalButton.gameObject.SetActive(false);
        _takeBonusButton.gameObject.SetActive(false);
        _takeDoubleBonusButton.gameObject.SetActive(true);

        _getItemText.text = LocalizationHelper.GetLocale("window_workshop_collect_doubling_button");

        var parameters = new CustomEventParameters
        {
            Int = new Dictionary<string, int>
            {
                {"spent_crystals", 0 },
            }
        };
        App.Instance.Services.AnalyticsService.CustomEvent("complete_autominer_double", parameters);

        Double();

    }

    public void Double()
    {

        _isDoubled = true;

        _dropItemsContainer.ClearChildObjects();

        _dropPack.Multiplay(2);

        foreach (var item in _dropPack.Items)
        {
            if (item.Amount == 0) continue;
            var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
            itemView.Initialize(item, Color.green);
        }
    }
}
