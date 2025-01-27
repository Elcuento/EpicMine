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

public class WindowCrystalSubscriptionStartGameOffer : WindowBase
{
    public const string ShopPackIdWeekProduct = "shop_pack_stock_crystals2";

    [SerializeField] private TextMeshProUGUI _instantGet;
    [SerializeField] private TextMeshProUGUI _totalGet;

    [Header("Free/Week")]
    [SerializeField] private GameObject _weekProduct;
    [SerializeField] private TextMeshProUGUI _weekPrice;

    [Space]
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _luaPp;

    [Space]
    [SerializeField] private GameObject _top;
    [SerializeField] private GameObject _bottom;
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


        _bottom.SetActive(true);
        _luaPp.gameObject.SetActive(true);

        if (_weekProductData != null)
        {
            var pack = ShopHelper.GetPackByProductId(_weekProductData);

            _weekPrice.text = string.Format($"{LocalizationHelper.GetLocale("window_crystals_subscription_offer_free_week_offer_disc")}",
                $"{_weekProductData.metadata.localizedPrice:0.##} {_weekProductData.metadata.isoCurrencyCode}");

            var luaLink = $"<link=\"http://www.blacktemple.ru/terms-of-use \"><color=#FFFFFF><u>{ LocalizationHelper.GetLocale("window_crystals_subscription_offer_lua") }</u></color></link>";
            var ppLink = $"<link=\"http://www.blacktemple.ru/privacy-policy \"><color=#FFFFFF><u>{ LocalizationHelper.GetLocale("window_crystals_subscription_offer_pp") }</u></color></link>";
            var weekPrice = $"{_weekProductData.metadata.localizedPrice:0.##} {_weekProductData.metadata.isoCurrencyCode}";

            _description.text =
                string.Format(LocalizationHelper.GetLocale("window_crystals_subscription_offer_description"), weekPrice);

            _luaPp.text = string.Format(LocalizationHelper.GetLocale("window_crystals_subscription_offer_lua_pp"), luaLink, ppLink);
            _luaPp.fontSizeMax = _description.fontSizeMax;

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

        LayoutRebuilder.ForceRebuildLayoutImmediate(_description.GetComponent<RectTransform>());
    }

    private void ShowCloseButton()
    {
        _closeButton.SetActive(true);
    }

    public void OnClickDescription()
    {
  
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(_luaPp, Input.mousePosition, GetComponent<Canvas>().worldCamera);

        if (linkIndex == -1)
            return;

        var linkInfo = _luaPp.textInfo.linkInfo[linkIndex];

        Application.OpenURL(linkInfo.GetLinkID().Replace(" ", ""));

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
