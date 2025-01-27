using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowShopCrystalItem : MonoBehaviour
    {
        public ShopPack ShopPack { get; private set; }

        [Space]
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _costText;

        [Header("Events")]
        [SerializeField] private GameObject _blackFridayContent;
        [SerializeField] private TextMeshProUGUI _blackFridayTitle;

        [Space]
        [SerializeField] private RectTransform _iconRectTransform;
        [SerializeField] private RectTransform _rect;

        [Header("Sale")]
        [SerializeField] private GameObject _saleContent;
        [SerializeField] private TextMeshProUGUI _oldCostText;
        [SerializeField] private TextMeshProUGUI _saleRedLentLabel;
        [SerializeField] private TextMeshProUGUI _redLentSale;

        [Space]
        [SerializeField] private GameObject _popularLent;

        [Space]
        private Product _product;


        public void Initialize(ShopPack staticShopPack, Product product)
        {
            ShopPack = staticShopPack;
            _product = product;

            var amount = ShopPack.Currency[CurrencyType.Crystals];

            _title.text = string.Format(LocalizationHelper.GetLocale(ShopPack.Id), amount);

            _costText.text = $"{_product.metadata.localizedPrice:0.##} {product.metadata.isoCurrencyCode}";

            var sprite = SpriteHelper.GetShopPackImage(ShopPack.Id);

            _iconRectTransform.anchoredPosition = Vector2.zero;
            _icon.sprite = sprite;


            _saleContent.SetActive(ShopPack.SalePercent > 0);
            _saleRedLentLabel.text = (-ShopPack.SalePercent) + "%";
            var oldPrice = (int) ((int) _product.metadata.localizedPrice +
                                  (int) _product.metadata.localizedPrice * (ShopPack.SalePercent * 0.01));
            _oldCostText.text = $"{oldPrice:0.##} {product.metadata.isoCurrencyCode}";

            _popularLent.SetActive(ShopPack.Id == "shop_pack_crystals_3");

            if (App.Instance.GameEvents.IsActive(GameEventType.BlackFriday))
            {
                _blackFridayContent.SetActive(true);
                _blackFridayTitle.text =
                    string.Format(" + " + LocalizationHelper.GetLocale("black_friday_window_shop_item_price"),
                        amount / 2);
            }

        }


        public void Click()
        {
            App.Instance.Player.Shop.BuyShopPack(_product);
        }
    }
}


/*
var windowPopup = WindowManager.Instance.Show<WindowShopPopup>();
if (ShopPack != null)
    windowPopup.Initialize(ShopPack);
else if (_product != null)*/
