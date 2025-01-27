using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowCollectWorkshopSlot : WindowBase
    {
        [SerializeField] private ItemView _itemView;

        [Space]
        [SerializeField] private Image _costIcon;
        [SerializeField] private TextMeshProUGUI _costText;

        [Space]
        [SerializeField] private GameObject _adsButton;
        [SerializeField] private GameObject _crystalButton;
        [SerializeField] private TextMeshProUGUI _crystalButtonConstText;

        [Space]
        [SerializeField] private TextMeshProUGUI _showedText;
        [SerializeField] private TextMeshProUGUI _collectButtonText;

        private Core.WorkshopSlot _workshopSlot;
        private bool _adsShowed;
        private bool _crystalPayed;
        private int _amount;
        private int _itemPrice;


        public void Initialize(Core.WorkshopSlot workshopSlot)
        {
            _workshopSlot = workshopSlot;
            _amount = workshopSlot.CompleteAmount;

            _itemPrice = StaticHelper.GetItemPrice(_workshopSlot.StaticRecipe.Id);
            _collectButtonText.text = LocalizationHelper.GetLocale("window_workshop_collect_button");
            _crystalButtonConstText.text = App.Instance.StaticData.Configs.Workshop.CollectCrystalPrice.ToString();

            var amount = GetTotalAmount();

            _itemView.Initialize(_workshopSlot.StaticRecipe.Id, amount, App.Instance.Player.Workshop.ResourceCoefficient > 1 ? "#00c3ff" : "#FFFFFF");
            _costText.text = (_itemPrice * amount).ToString();

            _costIcon.gameObject.SetActive(_itemPrice > 0);
            _costText.gameObject.SetActive(_itemPrice > 0);
        }

        public int GetTotalAmount()
        {
            var adCrystalCoefficient = _adsShowed || _crystalPayed ? 2 : 1;
            var boostM = App.Instance.Player.Workshop.ResourceCoefficient;

            var amount = boostM * _amount * adCrystalCoefficient *
                         _workshopSlot.StaticRecipe.Amount;

            return amount;
        }

        public void Double()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.MultiplySmeltedResources);
        }

        public void Collect()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _workshopSlot.CollectCompleted(_amount, _adsShowed , _crystalPayed);
            WindowManager.Instance.Close(this, withDestroy: true);
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnAdRewardedVideoCompleted;
        }

        private void OnAdRewardedVideoCompleted(AdSource adSource, bool isShowed, string rewardId, int rewardAmount)
        {
            Debug.Log("newzeal " + isShowed +":" + adSource);

            if (isShowed && adSource == AdSource.MultiplySmeltedResources)
            {
                _crystalButton.SetActive(false);
                _adsButton.SetActive(false);
                _adsShowed = true;
                _showedText.gameObject.SetActive(true);
                _showedText.text = LocalizationHelper.GetLocale("window_workshop_collect_ads_showed");

                var amount = GetTotalAmount();

                _itemView.Initialize(_workshopSlot.StaticRecipe.Id, amount);
                _itemView.SetColor(Color.green);
                _collectButtonText.text = LocalizationHelper.GetLocale("window_workshop_collect_doubling_button");
                _costText.text = (_itemPrice * amount).ToString();
            }
        }

        public void OnClickDoubleCrystal()
        {

            if (_adsShowed || _crystalPayed)
                return;

            if (!App.Instance.Player.Wallet.Has(new Currency(CurrencyType.Crystals,
                App.Instance.StaticData.Configs.Workshop.CollectCrystalPrice)))
            {
                WindowManager.Instance.Show<WindowShop>()
                    .OpenCrystals();

                return;
            }

            var staticData = App.Instance.StaticData;

            var staticWorkshopSlotsCount = staticData.WorkshopSlots.Count;
            if (staticWorkshopSlotsCount <= _workshopSlot.Number)
            {
                Debug.LogError("Wrong slot");
                return;
            }

            var priceInCrystals = staticData.Configs.Workshop.CollectCrystalPrice;

            if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, priceInCrystals))
            {
                Debug.LogError("Not enough crystals");
                return;
            }

            OnGetCrystalPayResponse();
        }
        private void OnGetCrystalPayResponse()
        {
            _adsButton.SetActive(false);
            _crystalButton.SetActive(false);
            _crystalPayed = true;
            _showedText.gameObject.SetActive(true);
            _showedText.text = LocalizationHelper.GetLocale("window_workshop_collect_crystal_pay");
            _itemView.SetColor(Color.green);
            _collectButtonText.text = LocalizationHelper.GetLocale("window_workshop_collect_doubling_button");

            var amount = GetTotalAmount();

            _itemView.Initialize(_workshopSlot.StaticRecipe.Id, amount);

            _costText.text = (_itemPrice * amount).ToString();

        }

        public override void OnClose()
        {
            base.OnClose();
            Unsubscribe();
            _amount = 0;
            _crystalPayed = false;
            _adsShowed = false;
            _adsButton.SetActive(true);
            _showedText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (App.Instance != null)
                App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnAdRewardedVideoCompleted;
        }
    }
}