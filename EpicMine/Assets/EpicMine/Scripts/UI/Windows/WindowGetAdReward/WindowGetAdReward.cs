using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowGetAdReward : WindowBase
{
    private Action _onClose;

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _charges;

    private ShopPack _shopPack;
    private ShopTimerPurchase _timePurchase;

    public void Start()
    {
        App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnAdRewardedVideoCompleted;
    }
    public void Initialize(ShopTimerPurchase timerPurchase)
    {
        _timePurchase = timerPurchase;


        _title.text = string.Format(LocalizationHelper.GetLocale(_shopPack.Id), _shopPack.Currency.FirstOrDefault(x => x.Key == CurrencyType.Crystals).Value);
        _charges.text = $"{LocalizationHelper.GetLocale("left")} {(_timePurchase?.Charge ?? _shopPack.Charge)}";


        EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
        EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
    }

    public void OnDestroy()
    {
        if (App.Instance == null)
            return;

        App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnAdRewardedVideoCompleted;
    }

    public void Initialize(ShopPack pack, ShopTimerPurchase timerPurchase, Action onClose)
    {
        _onClose = onClose;
        _shopPack = pack;
        _timePurchase = timerPurchase;


        _title.text = string.Format(LocalizationHelper.GetLocale(_shopPack.Id), _shopPack.Currency.FirstOrDefault(x=>x.Key == CurrencyType.Crystals).Value );
        _charges.text = $"{LocalizationHelper.GetLocale("left")} {(_timePurchase?.Charge ?? _shopPack.Charge)}";


        EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
        EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
    }

    public void OnClickAd()
    {
        BuyByAd();
    }

    private void BuyByAd()
    {
        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
        
#if UNITY_EDITOR
        OnAdRewardedVideoCompleted(AdSource.CrystalAdReward, true, "", 0); // TODO
#else
            App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.CrystalAdReward);
#endif

    }

    private void OnAdRewardedVideoCompleted(AdSource source, bool isShowed, string rewardId, int rewardAmount)
    {
        if (!isShowed)
            return;

        if (source != AdSource.CrystalAdReward)
            return;

        App.Instance.Player.Shop.BuyShopPack(_shopPack);
    }

    private void OnBuyShopPack(ShopBuyShopPackEvent eventData)
    {
        if (eventData.ShopPack.Id != _shopPack.Id)
            return;

        _timePurchase = App.Instance.Player.Shop.TimePurchase.Find(x => x.Id == _shopPack.Id);

        if (_timePurchase != null && _timePurchase.Charge > 0)
        {
            Initialize(_timePurchase);
        }
        else
        {
            Close();
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
        _onClose?.Invoke();
    }
}
