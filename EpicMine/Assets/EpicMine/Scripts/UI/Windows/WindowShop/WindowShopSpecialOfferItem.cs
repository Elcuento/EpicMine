using System;
using System.Collections.Generic;
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

public class WindowShopSpecialOfferItem : MonoBehaviour
{
    private Action _onDestroy;

    [SerializeField] private TextMeshProUGUI _header;

    [Header("Crystals")]
    [SerializeField] private GameObject _crystalsContainer;
    [SerializeField] private Image _crystalsPicture;
    [SerializeField] private TextMeshProUGUI _crystalsCount;

    [Header("Items")]
    [SerializeField] private GameObject _itemsContainer;

    [SerializeField] private TextMeshProUGUI _item1Count;
    [SerializeField] private TextMeshProUGUI _item2Count;
    [SerializeField] private TextMeshProUGUI _item3Count;

    [Header("Boosters")]
    [SerializeField] private GameObject _boosterContainer;
    [SerializeField] private  Image _boosterPicture;
    [SerializeField] private TextMeshProUGUI _boosterTime;
    [SerializeField] private TextMeshProUGUI _boosterName;

    [Header("Dynamite")]
    [SerializeField] private GameObject _dynamiteContainer;
    [SerializeField] private Image _dynamitePicture;
    [SerializeField] private TextMeshProUGUI _dynamiteAmount;

    [Space]
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private GameObject _oldPriceContainer;
    [SerializeField] private TextMeshProUGUI _oldPrice;
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private TextMeshProUGUI _sale;

    [SerializeField] private ShopSpecialOfferType _type;


    private ShopPack _pack;
    private ShopOffer _offer;
    private Product _product;

    public void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

            EventManager.Instance.Unsubscribe<UnscaledSecondsTickEvent>(OnTickEvent);
            EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuy);
    }

    public void TimeStart()
    {
        EventManager.Instance.Subscribe<UnscaledSecondsTickEvent>(OnTickEvent);
        OnTickEvent(new UnscaledSecondsTickEvent());
    }

    public void TimeOff(bool initialize = false)
    {
        EventManager.Instance.Unsubscribe<UnscaledSecondsTickEvent>(OnTickEvent);

        _onDestroy?.Invoke();
        Destroy(gameObject);
    }

    public void OnTickEvent(UnscaledSecondsTickEvent data)
    {

        if (_offer == null || _offer.IsCompleted || !_offer.IsActive)
        {
            TimeOff();
            return;
        }

        var date = new DateTime();
        date = date.AddSeconds(_offer.TimeLeft);
        _timer.text = date.ToString("HH:mm:ss", CultureInfo.InvariantCulture); 
    }

    public void Initialize(ShopOffer offer, Action onBuy)
    {
        _onDestroy = onBuy;

        _pack = offer.StaticShopPack;
        _offer = offer;
        _product = InAppPurchaseManager.Instance.StoreController.products.all.FirstOrDefault(x => x.definition.id == $"{_pack.Id}");

        var price = (double)_product.metadata.localizedPrice;
        _price.text = $"{price:0.##} {_product.metadata.isoCurrencyCode}";

        _header.text = LocalizationHelper.GetLocale(_pack.Id);

        _type = _pack.Items.ContainsKey("tnt_1") ? ShopSpecialOfferType.Dynamite
            : _pack.Currency.ContainsKey(CurrencyType.Crystals) ? ShopSpecialOfferType.Crystal
            : _pack.Buffs.Count > 0 ? ShopSpecialOfferType.Booster : ShopSpecialOfferType.Potions;


        switch (_type)
        {
            case ShopSpecialOfferType.Crystal:
                _crystalsCount.text = _pack.Currency[CurrencyType.Crystals].ToString();
                break;
            case ShopSpecialOfferType.Booster:
                var booster = _pack.Buffs.First();
                var buffBust = App.Instance.StaticData.Buffs.Find(x => x.Id == booster);
                _boosterTime.text = (buffBust.Time/24) + LocalizationHelper.GetLocale("days");

                if (buffBust.Value.ContainsKey(BuffValueType.Melting))
                {
                    _boosterPicture.sprite = SpriteHelper.GetShopPackImage("shop_pack_workshop_booster_3");
                    _boosterName.text = LocalizationHelper.GetLocale("shop_pack_workshop_booster_3");
                }
                
                else if (buffBust.Value.ContainsKey(BuffValueType.Resource))
                {
                    _boosterPicture.sprite = SpriteHelper.GetShopPackImage("shop_pack_resource_booster_3");
                    _boosterName.text = LocalizationHelper.GetLocale("shop_pack_resource_booster_3");
                }

                break;
            case ShopSpecialOfferType.Potions:
                var items = _pack.Items.Values.ToList();
                _item1Count.text = items[0].ToString();
                _item2Count.text = items[1].ToString();
                _item3Count.text = items[2].ToString();
                break;
            case ShopSpecialOfferType.Dynamite:
                _dynamiteAmount.text =
                    $"{LocalizationHelper.GetLocale("tnt_1")} {_pack.Items.First().Value} {LocalizationHelper.GetLocale("amount")}";
                break;
        }

        var oldPrice = (price + price * (_pack.SalePercent * 0.01));
        _oldPrice.text = $"{oldPrice:0.##} {_product.metadata.isoCurrencyCode}";
        _sale.text = $"{LocalizationHelper.GetLocale("sale")} {_pack.SalePercent}%";

        _boosterContainer.SetActive(_type == ShopSpecialOfferType.Booster);
        _itemsContainer.SetActive(_type == ShopSpecialOfferType.Potions);
        _dynamiteContainer.SetActive(_type == ShopSpecialOfferType.Dynamite);
        _crystalsContainer.SetActive(_type == ShopSpecialOfferType.Crystal);

        TimeStart();
    }

    public void Buy()
    {
        App.Instance.Player.Shop.BuyShopPack(_product);
    }

    public void OnBuy(ShopBuyShopPackEvent eventData)
    {
        if (eventData.ShopPack.Id != _product.definition.id)
            return;

        _onDestroy?.Invoke();
      //  Destroy(gameObject);

    }
}
