using System;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class WindowCrystalSubscriptionStartGameOfferAndroid : WindowBase
{
    public const string ShopPackIdWeekProduct = "shop_pack_stock_crystals2";

    [SerializeField] private TextMeshProUGUI _instantGet;
    [SerializeField] private TextMeshProUGUI _totalGet;

    [Header("Free/Week")]
    [SerializeField] private GameObject _weekProduct;
    [SerializeField] private TextMeshProUGUI _weekPrice;

    [Space]
    [SerializeField] private GameObject _top;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private CanvasGroup _closeButtonCanvasGroup;

    private Product _weekProductData;
    private Action _onClose;

    private void Start()
    {
        EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnShopPackBrought);
    }

    public void Initialize(Action onClose = null)
    {
        Clear();

        _onClose = onClose;

        _weekProductData = ShopHelper.GetProductByPackId(ShopPackIdWeekProduct);

        if (_weekProductData != null)
        {
            var pack = ShopHelper.GetPackByProductId(_weekProductData);

            _weekPrice.text = string.Format($"{LocalizationHelper.GetLocale("window_crystals_subscription_offer_free_week_offer_disc")}",
                $"{_weekProductData.metadata.localizedPrice:0.##} {_weekProductData.metadata.isoCurrencyCode}");

            if (pack.Buffs.Count > 0)
            {
                var staticBuff = App.Instance.StaticData.Buffs.Find(x => x.Id == pack.Buffs[0]);

                var everyDay = staticBuff.Value.FirstOrDefault(x => x.Key == BuffValueType.CrystalsByDay).Value;
                var monthDays = (DateTime.UtcNow.AddMonths(1) - DateTime.UtcNow).Days;
                var total = pack.Currency.FirstOrDefault(x => x.Key == CurrencyType.Crystals)
                                .Value + monthDays * everyDay;

                _instantGet.text = everyDay.ToString();
                _totalGet.text = total.ToString();
            }
        }
        else
        {
            _weekProduct.SetActive(false);
            Close();
        }

        _closeButtonCanvasGroup.DOFade(1, 1).OnComplete(ShowCloseButton).SetDelay(2).SetUpdate(true);
    }

    private void ShowCloseButton()
    {
        _closeButton.SetActive(true);
    }


    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnShopPackBrought);
    }

    private void OnShopPackBrought(ShopBuyShopPackEvent pack)
    {
        if (pack.ShopPack != null && (pack.ShopPack.Id == ShopPackIdWeekProduct))
        {
            Close();
        }
    }

    public override void OnClose()
    {
        _onClose?.Invoke();
        base.OnClose();
    }

    private void Clear()
    {
        _closeButton.SetActive(false);
        _closeButtonCanvasGroup.alpha = 0;
    }

    public void OnClickWeek()
    {
        App.Instance.Player.Shop.BuyShopPack(_weekProductData);
    }
}
