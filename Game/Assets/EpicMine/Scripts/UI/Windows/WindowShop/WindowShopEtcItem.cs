using System;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;

using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowShopEtcItem : MonoBehaviour
    {
        public ShopPack ShopPack { get; private set; }

        [SerializeField] private Image _background;
        [SerializeField] private Image _icon;
        [SerializeField] private RectTransform _iconRectTransform;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Image _costIcon;
        [SerializeField] private GameObject _redDot;
        [SerializeField] private RectTransform _rect;

        private Product _product;


        public void Start()
        {
            EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
        }

        public void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
        }

        public void OnBuyShopPack(ShopBuyShopPackEvent eventData)
        {
            if (eventData.ShopPack.Id == ShopPack.Id)
            {
                Initialize(ShopPack);
            }
        }

        public void Initialize(ShopPack staticShopPack)
        {

            ShopPack = staticShopPack;
            _costText.text = ShopPack.CrystalCost.ToString();

            var sprite = staticShopPack.Type == ShopPackType.Hilt ||
                          staticShopPack.Type == ShopPackType.ResourceBooster ||
                          staticShopPack.Type == ShopPackType.WorkShopBooster ?
                SpriteHelper.GetShopPackImage(ShopPack) :
                SpriteHelper.GetShopPackImage(ShopPack.Id);


            var pivotX = sprite.pivot.x / sprite.textureRect.width;
            var pivotY = sprite.pivot.y / sprite.textureRect.height;
            _iconRectTransform.pivot = new Vector2(pivotX, pivotY);
           // _iconRectTransform.anchoredPosition = Vector2.zero;
            _icon.sprite = sprite;
          //  _icon.SetNativeSize();

            var amount = 0;

            if (ShopPack.Items.Count > 0)
            {
                var firstItem = ShopPack.Items.First();
                amount = firstItem.Value;
            }

            if (staticShopPack.Type == ShopPackType.ResourceBooster ||
                staticShopPack.Type == ShopPackType.WorkShopBooster)
            {
                _title.text = LocalizationHelper.GetLocale(ShopPack.Id);
            }
            else
            {
                _title.text = string.Format(LocalizationHelper.GetLocale(ShopPack.Id), amount);
            }
        }

        public void Click()
        {
            var cost = new Currency(CurrencyType.Crystals, ShopPack.CrystalCost);

            if (!App.Instance.Player.Wallet.Has(cost))
            {
                WindowManager.Instance.Show<WindowNotEnoughCurrency>()
                    .Initialize("window_not_enough_currency_crystals_discription", "window_not_enough_currency_buy",
                        WindowManager.Instance.Show<WindowShop>()
                            .OpenCrystals);
                return;
            }

            App.Instance.Player.Shop.BuyShopPack(ShopPack);

        }
    }
}