using System;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class WindowForceAds : WindowBase
{
    private Action _onClose;
    [SerializeField] private TextMeshProUGUI _title;

    [SerializeField] private TextMeshProUGUI _buttonPrice;
    [SerializeField] private TextMeshProUGUI _buttonTitle;

    private Product _product;

    public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
    {
        base.OnShow(withPause, withCurrencies, withRating);

        if (!InAppPurchaseManager.Instance.IsInitialized || App.Instance.Services.AdvertisementService.GetCurrentForceAdsPeriod() != 0)
        {
            Close();
            return;
        }

        UnSubscribe();
        Subscribe();

        var shopPack =
            App.Instance.StaticData.ShopPacks.Find(x =>
                x.Id == App.Instance.StaticData.Configs.Ads.AdsUnlockShopPackId);

        _product = ShopHelper.GetProductByPackId(App.Instance.StaticData.Configs.Ads.AdsUnlockShopPackId);

        if (_product == null)
        {
            App.Instance.Services.LogService.LogError("Force ads product is null");
            Close();
            return;
        }
        if (shopPack == null )
        {
            App.Instance.Services.LogService.LogError("Force ads shop pack is null");
            Close();
            return;
        }

        if (!shopPack.Currency.ContainsKey(CurrencyType.Crystals))
            return;

        var crystals = shopPack.Currency[CurrencyType.Crystals];
        
        _buttonPrice.text = $"{_product.metadata.localizedPrice:0.##} {_product.metadata.isoCurrencyCode}";
        _buttonTitle.text = string.Format(LocalizationHelper.GetLocale("window_force_ads_button_title"), crystals);

    }

    public void OnClickButton()
    {
        App.Instance.Services.AnalyticsService.CustomEvent("ads_disable_click", new CustomEventParameters
        {});
        App.Instance.Player.Shop.BuyShopPack(_product);
    }

    public void Initialize(Action onClose)
    {
        _onClose = onClose;
    }

    public override void OnClose()
    {
        base.OnClose();
        _onClose?.Invoke();
        UnSubscribe();
    }


    public void OnBuy(ShopBuyShopPackEvent eventData)
    {
        if (eventData.ShopPack.Id != _product.definition.id)
            return;

        Close();

    }

    private void Subscribe()
    {
        EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBuy);
    }

    private void UnSubscribe()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuy);
        _onClose = null;
    }

    public void OnDestroy()
    {
        UnSubscribe();
    }
}
