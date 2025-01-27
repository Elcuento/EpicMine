using System;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;


public class WindowCrystalSubscriptionOffer : WindowBase
{
    public const string ShopPackIdYearProduct = "shop_pack_stock_crystals1";
    public const string ShopPackIdMonthProduct = "shop_pack_stock_crystals0";
    public const string ShopPackIdWeekProduct = "shop_pack_stock_crystals2";

    [Space]
    [SerializeField] private TextMeshProUGUI _instantGet;
    [SerializeField] private TextMeshProUGUI _totalGet;

    [Header("Year")]
    [SerializeField] private GameObject _yearProduct;
    [SerializeField] private TextMeshProUGUI _yearPrice;
    [SerializeField] private TextMeshProUGUI _yearBonus;


    [Header("Month")]
    [SerializeField] private GameObject _monthProduct;
    [SerializeField] private TextMeshProUGUI _monthPrice;
    [SerializeField] private TextMeshProUGUI _monthBonus;

    [Header("Free/Week")]
    [SerializeField] private GameObject _weekProduct;
    [SerializeField] private TextMeshProUGUI _weekPrice;

    [Space]
    [SerializeField] private RectTransform _bottomContent;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _luaPp;

    [Space]
    [SerializeField] private GameObject _top;
    [SerializeField] private GameObject _bottom;

    private Product _yearProductData;
    private Product _monthProductData;
    private Product _weekProductData;

    private void Start()
    {
        EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnShopPackBrought);
    }

    public void Initialize()
    {
        _monthProductData = ShopHelper.GetProductByPackId(ShopPackIdMonthProduct);

        _yearProductData = ShopHelper.GetProductByPackId(ShopPackIdYearProduct);

        _weekProductData = ShopHelper.GetProductByPackId(ShopPackIdWeekProduct);

        _bottom.SetActive(App.Instance.CurrentPlatform == PlatformType.IOS || Application.platform == RuntimePlatform.WindowsEditor);

        if (_monthProductData != null)
        {
            var pack = ShopHelper.GetPackByProductId(_monthProductData);

            _monthBonus.text =
                string.Format($"{LocalizationHelper.GetLocale("window_crystals_subscription_offer_bonus")}", "100");
            _monthPrice.text = $"{_monthProductData.metadata.localizedPrice:0.##} {_monthProductData.metadata.isoCurrencyCode}";

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
        else _monthProduct.SetActive(false);

        if (_yearProductData != null)
        {
            _yearPrice.text = $"{_yearProductData.metadata.localizedPrice:0.##} {_yearProductData.metadata.isoCurrencyCode}";
            _yearBonus.text =
                string.Format($"{LocalizationHelper.GetLocale("window_crystals_subscription_offer_bonus")}", "100");
        }
        else _yearProduct.SetActive(false);

        if (_weekProductData != null)
        {
            _weekPrice.text = string.Format($"{LocalizationHelper.GetLocale("window_crystals_subscription_offer_free_week_offer_disc")}",
                $"{_weekProductData.metadata.localizedPrice:0.##} {_weekProductData.metadata.isoCurrencyCode}");

            var luaLink = $"<link=\"https://www.blacktemple.ru/terms-of-use \"><color=#FFFFFF><u>{LocalizationHelper.GetLocale("window_crystals_subscription_offer_lua")}</u></color></link>";
            var ppLink = $"<link=\"https://www.blacktemple.ru/privacy-policy \"><color=#FFFFFF><u>{LocalizationHelper.GetLocale("window_crystals_subscription_offer_pp")}</u></color></link>";
            var weekPrice = $"{_weekProductData.metadata.localizedPrice:0.##} {_weekProductData.metadata.isoCurrencyCode}";

            _description.text =
                string.Format(LocalizationHelper.GetLocale("window_crystals_subscription_offer_description"), weekPrice);

            _luaPp.text = string.Format(LocalizationHelper.GetLocale("window_crystals_subscription_offer_lua_pp"), luaLink, ppLink);
            _luaPp.fontSizeMax = _description.fontSizeMax;
        }
        else _weekProduct.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_bottomContent);
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
        if (pack.ShopPack != null && (pack.ShopPack.Id == ShopPackIdYearProduct ||
                                      pack.ShopPack.Id == ShopPackIdMonthProduct ||
                                      pack.ShopPack.Id == ShopPackIdWeekProduct))
        {
            Close();
        }
    }

    public void OnClickMonth()
    {
        App.Instance.Player.Shop.BuyShopPack(_monthProductData);
    }

    public void OnClickYear()
    {
        App.Instance.Player.Shop.BuyShopPack(_yearProductData);
    }

    public void OnClickWeek()
    {
        App.Instance.Player.Shop.BuyShopPack(_weekProductData);
    }
}
