using System;
using System.Globalization;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class WindowShopSalePackItem : MonoBehaviour
{
    [SerializeField] private WindowShopSalePackResourceItem _itemPrefab;
    [SerializeField] private ScrollRect _itemScroll;

    [SerializeField] private Image _header;
    [SerializeField] private Image _lent_1;
    [SerializeField] private Image _lent_2;
    [SerializeField] private Image _shadowGradient;
    [SerializeField] private TextMeshProUGUI _headerLabel;

    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private TextMeshProUGUI _salePercent;

    [SerializeField] private TextMeshProUGUI _timeLeft;
    [SerializeField] private TextMeshProUGUI _charge;

    [SerializeField] private TextMeshProUGUI _crystalsGet;
    [SerializeField] private TextMeshProUGUI _crystalsGetFromResources;


    [SerializeField] private Color[] _packColors;
    [SerializeField] private Color[] _packTextColors;
    [SerializeField] private Color[] _packTailColors;

    private ShopSale _salePack;
    private ShopPack _shopPack;
    private Product _product;

    private void Start()
    {
        EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBuy);
    }


    public void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuy);
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
    }

    public void OnTickEvent(SecondsTickEvent data)
    {
        if (_salePack == null || !_salePack.IsActive)
        {
            TimeOff();
            return;
        }

        var date = new DateTime();
        date = date.AddSeconds(_salePack.TimeLeft);
        _timeLeft.text = $"{LocalizationHelper.GetLocale("window_shop_time_left")} {date.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}";
    }

    public void TimeStart()
    {
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
        EventManager.Instance.Subscribe<SecondsTickEvent>(OnTickEvent);
        OnTickEvent(new SecondsTickEvent());
    }

    public void TimeOff(bool initialize = false)
    {
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);

        if (initialize)
            Initialize(_shopPack.Type);
    }


    public void Initialize(ShopPackType type)
    {
        if (type != ShopPackType.Alchemy && type != ShopPackType.Dragon && type != ShopPackType.Miner)
        {
            gameObject.SetActive(false);
            return;
        }


        _salePack = App.Instance.Player.Shop.ShopSale.Find(x => x.Type == type);


        var charge = _salePack != null && _salePack.IsActive ? _salePack.Charge : 1;
        var basePackId = $"shop_pack_{ type.ToString().ToLower() }";
        _shopPack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == $"{basePackId}_{charge}");
       
        _header.sprite = SpriteHelper.GetShopPackImage(basePackId);
        _headerLabel.text = LocalizationHelper.GetLocale(basePackId);

        _product = InAppPurchaseManager.Instance.StoreController.products.all.FirstOrDefault(x => x.definition.id == $"{_shopPack.Id}");

        if (_product == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _price.text = $"{_product.metadata.localizedPrice:0.##} {_product.metadata.isoCurrencyCode}";

        _salePercent.text = $"{_shopPack.SalePercent}%";
        _timeLeft.text = "";
        _charge.text = string.Format($"{LocalizationHelper.GetLocale("window_shop_sale_pack_charge")}", (charge));
        _crystalsGet.text = $"{LocalizationHelper.GetLocale("get")} {_shopPack.Currency[CurrencyType.Crystals]}";
    
        var items = ShopHelper.GetShopPackItems(_shopPack);
        long.TryParse(_shopPack.ExtraAttribute, out var crystalCost);
        _crystalsGetFromResources.text = $"{LocalizationHelper.GetLocale("window_shop_get_resource_price")} <color=#FFFFFF>{crystalCost}</color>";

        _itemScroll.content.ClearChildObjects();

        foreach (var packItem in items)
        {
            var item = Instantiate(_itemPrefab, _itemScroll.content, false);
            item.Initialize(packItem.Key, packItem.Value);
        }

        foreach (var i in _shopPack.Currency)
        {
            var itemIcon = i.Key == CurrencyType.Crystals
                ? App.Instance.ReferencesTables.Sprites.CrystalsIcon
                : App.Instance.ReferencesTables.Sprites.GoldIcon;
            var itemName = i.Key == CurrencyType.Crystals ? "window_shop_crystals" : "window_shop_gold";

            var item = Instantiate(_itemPrefab, _itemScroll.content, false);
            item.Initialize(itemName, itemIcon, i.Key == CurrencyType.Gold ?
                ShopHelper.GetCurrentGoldShopPackAmount(i.Value) : i.Value);
        }

        var colorType = _shopPack.Type == ShopPackType.Miner ? 0 :
            _shopPack.Type == ShopPackType.Alchemy ? 1 : 2;

        _lent_1.color = _packTailColors[colorType];
        _lent_2.color = _packColors[colorType];
        _crystalsGetFromResources.color = _packTextColors[colorType];

        if (_salePack == null || !_salePack.IsActive)
        {
            TimeOff();
        }
        else
        {
            TimeStart();
        }
    }

    public void OnScroll()
    {
        _shadowGradient.enabled = _itemScroll.verticalNormalizedPosition >= 0;
    }

    public void OnClickBuy()
    {
        App.Instance.Player.Shop.BuyShopPack(_product);       
    }

    public void OnBuy(ShopBuyShopPackEvent salePack)
    {
        if (salePack.ShopPack.Type != _shopPack.Type)
            return;

        _salePack = App.Instance.Player.Shop.ShopSale.Find(x=>x.Type == salePack.ShopPack.Type);

        TimeOff(true);
    }

}
