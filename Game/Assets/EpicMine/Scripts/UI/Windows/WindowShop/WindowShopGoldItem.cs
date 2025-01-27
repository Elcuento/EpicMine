using System;
using System.Globalization;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;

using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using ShopTimerPurchase = BlackTemple.EpicMine.Core.ShopTimerPurchase;

namespace BlackTemple.EpicMine
{
    public class WindowShopGoldItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        [SerializeField] private RectTransform _rect;
        [SerializeField] private RectTransform _iconRectTransform;

        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _costText;
       
        [SerializeField] private Button _buyButton;
        [SerializeField] private GameObject _adBuyButton;

        [SerializeField] private Image _saleLent;
        [SerializeField] private TextMeshProUGUI _saleLentValue;

        [SerializeField] private GameObject _saleStar;
        [SerializeField] private TextMeshProUGUI _saleStarValue;

        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private GameObject _goldTimerContent;

        [SerializeField] private TextMeshProUGUI _charge;

        public ShopPack ShopPack { get; private set; }
        private ShopTimerPurchase _timePurchase;



        public void Start()
        {
            EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
        }

        public void OnDestroy()
        {
            if (App.Instance != null)
            {
                App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnAdRewardedVideoCompleted;

            }

            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
            EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuyShopPack);

        }

        public void OnBuyShopPack(ShopBuyShopPackEvent eventData)
        {
            if (eventData.ShopPack.Id == ShopPack.Id)
            {
                _timePurchase = App.Instance.Player.Shop.TimePurchase.Find(x => x.Id == ShopPack.Id);
                Initialize(ShopPack, _timePurchase);
            }
        }

        public void TimeStart()
        {
            _goldTimerContent.SetActive(true);

            EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
            EventManager.Instance.Subscribe<SecondsTickEvent>(OnTickEvent);
            OnTickEvent(new SecondsTickEvent());
        }

        public void TimeOff(bool initialize)
        {
            EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);

            if (initialize)
                Initialize(ShopPack);
        }


        public void OnTickEvent(SecondsTickEvent data)
        {
            if (_timePurchase == null || _timePurchase.IsActive || _timePurchase.Charge > 0)
            {
                TimeOff(true);
                return;
            }

            var date = new DateTime();
            date = date.AddSeconds(_timePurchase.TimeLeft);
            _timer.text = date.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }


        public void Initialize(ShopPack staticShopPack, ShopTimerPurchase purchase = null)
        {
            Clear();

            ShopPack = staticShopPack;
            _timePurchase = purchase;

            long amount = 0;

            if (ShopPack.CrystalCost == 0)
            {
                var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number;
                amount = (int) (StaticHelper.GetChestGoldAmount(tier, false) * 0.5f);
            }
            else
            {
                amount = ShopHelper.GetCurrentGoldShopPackAmount(ShopPack.Currency[CurrencyType.Gold]);
            }

            _title.text = string.Format(LocalizationHelper.GetLocale(ShopPack.Id), amount);
            _costText.text = ShopPack.CrystalCost.ToString();


            var sprite = SpriteHelper.GetShopPackImage(ShopPack.Id);
            _icon.sprite = sprite;

            _adBuyButton.gameObject.SetActive(false);
            _buyButton.gameObject.SetActive(false);

            _saleStar.SetActive(ShopPack.SalePercent > 0);
            _saleStarValue.text = $"+{ShopPack.SalePercent}%";
            _saleLent.gameObject.SetActive(ShopPack.Id == "shop_pack_crystals_4" ||
                                           ShopPack.Id == "shop_pack_crystals_5" ||
                                           ShopPack.Id == "shop_pack_crystals_6");

            if (ShopPack.CrystalCost == 0)
            {
                _buyButton.gameObject.SetActive(false);
                _adBuyButton.gameObject.SetActive(_timePurchase == null ||
                                                  _timePurchase != null && _timePurchase.IsActive);

                _charge.text = $"{LocalizationHelper.GetLocale("left")} {_timePurchase?.Charge ?? ShopPack.Charge}";
            }
            else
            {

                _buyButton.gameObject.SetActive(
                    _timePurchase == null || _timePurchase != null && _timePurchase.IsActive);
                _adBuyButton.gameObject.SetActive(false);
            }

            if (_timePurchase != null && (!_timePurchase.IsActive && _timePurchase.Charge <= 0))
            {
                TimeStart();
            }

            if (ShopPack.CrystalCost == 0)
                App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnAdRewardedVideoCompleted;

        }

        private void Clear()
        {
            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnAdRewardedVideoCompleted;
        }

        public void Click()
        {
            if (ShopPack.CrystalCost == 0)
            {
                BuyByAd();
            }
            else
            {
                if (!App.Instance.Player.Wallet.Has(new Currency(CurrencyType.Crystals, ShopPack.CrystalCost)))
                {
                    WindowManager.Instance.Show<WindowNotEnoughCurrency>()
                        .Initialize("window_not_enough_currency_crystals_discription", "window_not_enough_currency_buy",
                            WindowManager.Instance.Show<WindowShop>().OpenCrystals);
                }
                else
                {
                    App.Instance.Player.Shop.BuyShopPack(ShopPack);
                }
            }

        }

        private void OnAdRewardedVideoCompleted(AdSource source, bool isShowed, string rewardId, int rewardAmount)
        {
            if (!isShowed)
                return;

             if (source != AdSource.GoldAdReward)
                return;

            App.Instance.Player.Shop.BuyShopPack(ShopPack);
        }


        public void BuyByAd()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

#if UNITY_EDITOR
            OnAdRewardedVideoCompleted(AdSource.GoldAdReward, true, "", 0); // TODO
#else
            App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.GoldAdReward);
#endif
        }
    }
}