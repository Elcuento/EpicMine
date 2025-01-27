using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;

using BlackTemple.EpicMine.Utils;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class WindowShop : WindowBase
    {
        [Space]
        [SerializeField] private ScrollRect _scrollRect;

        [Header("Items")]
        [SerializeField] private WindowShopGoldItem _itemGoldPrefab;
        [SerializeField] private WindowShopCrystalItem _itemCrystalPrefab;
        [SerializeField] private WindowShopPickaxeItem _itemPickaxePrefab;
        [SerializeField] private WindowShopResourceItem _itemResourcePrefab;
        [SerializeField] private WindowShopSpecialOfferItem _specialOfferPrefab;
        [SerializeField] private WindowShopSalePackItem _salePackItemPrefab;
        [SerializeField] private WindowShopEtcItem _etcItemPrefab;
        [SerializeField] private WindowShopEtcContainerItem _etcContainerPrefab;
        [SerializeField] private WindowShopEtcHeaderItem _etcHeaderPrefab;

        [Header("Contents")]
        [SerializeField] private GameObject _specialOfferEmptyContent;
        [SerializeField] private ScrollRect _specialOfferScroll;
        [SerializeField] private ScrollRect _salePackScroll;
        [SerializeField] private ScrollRect _resourceScroll;
        [SerializeField] private ScrollRect _goldScroll;
        [SerializeField] private ScrollRect _goldSubScroll;
        [SerializeField] private ScrollRect _crystalScroll;
        [SerializeField] private ScrollRect _crystalSubScroll;
        [SerializeField] private ScrollRect _etcScroll;

        [Header("Toggles")]
        [SerializeField] private Toggle _specialOfferToggle;
        [SerializeField] private Toggle _salePackToggle;
        [SerializeField] private Toggle _goldToggle;
        [SerializeField] private Toggle _crystalsToggle;
        [SerializeField] private Toggle _resourceToggle;
        [SerializeField] private Toggle _etcToggle;

        [Header("Headers")]
        [SerializeField] private TextMeshProUGUI _specialOfferLabel;
        [SerializeField] private TextMeshProUGUI _salePackLabel;
        [SerializeField] private TextMeshProUGUI _goldLabel;
        [SerializeField] private TextMeshProUGUI _crystalsLabel;
        [SerializeField] private TextMeshProUGUI _resourceLabel;
        [SerializeField] private TextMeshProUGUI _etcLabel;

        [Header("Special gold Offer")]
        [SerializeField] private WindowShopSpecialOfferGoldHeaderItem _specialOfferHeaderGoldItem;

        [Header("Special crystal Offer")]
        [SerializeField] private WindowShopSpecialOfferCrystalsHeaderItem _specialOfferHeaderCrystalItem;

        [Space]
        [SerializeField] private Color _activeToggleColor;
        [SerializeField] private Color _unActiveToggleColor;

        [Space]
        [SerializeField] private RedDotBaseView _goldRedDot;

        [Space]
        [SerializeField] private CanvasGroup _loadingIcon;

        private Action _onClose;
        private bool _isWideScreen;
        private float _soundTime;

        private void Start()
        {
            EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBroughtShopPack);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBroughtShopPack);
        }

        private void OnBroughtShopPack(ShopBuyShopPackEvent eventData)
        {
            if(eventData.ShopPack.Type == ShopPackType.Crystals)
            OnToggle();
        }


        public void OpenDefault()
        {
            _salePackToggle.isOn = !App.Instance.Player.Shop.ShopOffer.Any(x => x.IsActive);
            OnToggle();
        }

        public void OpenGold()
        {
            if (App.Instance.Player.Dungeon.LastOpenedTier.Number > 0)
            {
                _goldToggle.isOn = true;
                OnToggle();
            }else { OpenDefault(); }
        }

        public void OpenCrystals()
        {
            _crystalsToggle.isOn = true;
            OnToggle();
        }

        public void OpenTnt()
        {
            _etcToggle.isOn = true;
            OnToggle();
            LateTntScroll();
        }

        public void OpenBoost()
        {
            _etcToggle.isOn = true;
            OnToggle();
            LateBoostScroll();
        }

        public void OpenOffer()
        {
            _specialOfferToggle.isOn = true;
            OnToggle();
        }

        public void OpenSalePack()
        {
            _salePackToggle.isOn = true;
            OnToggle();
        }

        public void LateTntScroll()
        {
            StartCoroutine(ScrollTo(_etcScroll, 6, true));
        }
        public void LateBoostScroll()
        {
            StartCoroutine(ScrollTo(_etcScroll, 0, true));
        }


        public void OnToggle(bool state = true)
        {
            if (!state)
                return;

            Filter();

            _specialOfferScroll.content.transform.ClearChildObjects();
            _salePackScroll.content.transform.ClearChildObjects();
            _resourceScroll.content.transform.ClearChildObjects();
            _goldSubScroll.content.ClearChildObjects();
            _crystalSubScroll.content.ClearChildObjects();
            _etcScroll.content.ClearChildObjects();

            ShowLoading();

            if (_specialOfferToggle.isOn)
            {
                if (InAppPurchaseManager.Instance.IsInitialized)
                {
                    foreach (var statRes in App.Instance.Player.Shop.ShopOffer)
                    {
                        if (statRes.IsActive && !statRes.IsCompleted)
                        {
                            var res = Instantiate(_specialOfferPrefab, _specialOfferScroll.content.transform, false);
                            res.Initialize(statRes, OnBuySpecialOffer);
                        }
                    }
                    _specialOfferEmptyContent.SetActive(_specialOfferScroll.content.transform.childCount <= 0);
                    _specialOfferScroll.horizontalNormalizedPosition = 0;
                }
            }
            else if (_salePackToggle.isOn)
            {
                if (InAppPurchaseManager.Instance.IsInitialized)
                {

                    var minerPackInst = Instantiate(_salePackItemPrefab, _salePackScroll.content.transform, false);
                    minerPackInst.Initialize(ShopPackType.Miner);

                    var alchemyPackIns = Instantiate(_salePackItemPrefab, _salePackScroll.content.transform, false);
                    alchemyPackIns.Initialize(ShopPackType.Alchemy);

                    var dragonPackInst = Instantiate(_salePackItemPrefab, _salePackScroll.content.transform, false);
                    dragonPackInst.Initialize(ShopPackType.Dragon);

                    _salePackScroll.horizontalNormalizedPosition = 0;
                }

                /*  if (InAppPurchaseManager.Instance.IsInitialized)
                  {
                      foreach (var statRes in App.Instance.StaticData.ShopPacks)
                      {
                          if (statRes.Type == ShopPackType.Miner || statRes.Type == ShopPackType.Alchemy ||
                              statRes.Type == ShopPackType.Dragon)
                          {
                              var res = Instantiate(_salePackItemPrefab, _salePackScroll.content.transform, false);
                              res.Initialize(statRes, App.Instance.Player.Shop.ShopSale.Find(x => x.Id == statRes.Id));
                          }
                      }
                      _salePackScroll.horizontalNormalizedPosition = 0;
                  }*/
            }
            else if (_resourceToggle.isOn)
            {
                var allUnlockedItems = App.Instance.Player.Workshop.GetAllUnlockedItems();

                foreach (var statRes in App.Instance.StaticData.ShopResources)
                {
                    var res = Instantiate(_itemResourcePrefab, _resourceScroll.content.transform, false);
                    res.Initialize(statRes, !allUnlockedItems.Contains(statRes.Id));
                }
                _resourceScroll.verticalNormalizedPosition = 1;
            }
            else if (_goldToggle.isOn)
            {

                if (App.Instance.Player.Dungeon.LastOpenedTier.Number > 0)
                {
                    _specialOfferHeaderGoldItem.gameObject.SetActive(true);
                    _specialOfferHeaderGoldItem.Initialize();

                    foreach (var shopPack in App.Instance.StaticData.ShopPacks)
                    {
                        if (shopPack.Type == ShopPackType.Gold)
                        {
                            App.Instance.Controllers.RedDotsController.ViewShopPack(shopPack.Id);

                            var item = Instantiate(_itemGoldPrefab, _goldSubScroll.content.transform, false);
                            item.Initialize(shopPack,
                                App.Instance.Player.Shop.TimePurchase.Find(x => x.Id == shopPack.Id));
                        }
                    }
                    _goldSubScroll.horizontalNormalizedPosition = 0;
                    _goldRedDot.Hide();
                }
                else
                {
                    _specialOfferHeaderGoldItem.gameObject.SetActive(false);
                }

            }
            else if (_crystalsToggle.isOn)
            {
                if (InAppPurchaseManager.Instance.IsInitialized)
                {
                    _specialOfferHeaderCrystalItem.gameObject.SetActive(true);
                    _specialOfferHeaderCrystalItem.Initialize();

                    var sortedCrystals = App.Instance.StaticData.ShopPacks.Where(x => x.Type == ShopPackType.Crystals)
                        .OrderByDescending(x => x.Cost);

                    foreach (var shopPack in sortedCrystals)
                    {
                        var product = ShopHelper.GetProductByPackId(shopPack.Id);
                        if (product == null)
                            continue;

                        var item = Instantiate(_itemCrystalPrefab, _crystalSubScroll.content, false);
                        item.Initialize(shopPack, product);
                    }

                    _crystalSubScroll.horizontalNormalizedPosition = 0;
                }
                else
                {
                 _specialOfferHeaderCrystalItem.gameObject.SetActive(false);
                }
            }
            else if (_etcToggle.isOn)
            {
                // res boosters
                FillEtc(ShopPackType.ResourceBooster);

                // pickaxes
                Instantiate(_etcHeaderPrefab, _etcScroll.content.transform, false)
                    .Initialize(LocalizationHelper.GetLocale("Pickaxe"));

                var pickAxeTransform = Instantiate(_etcContainerPrefab, _etcScroll.content.transform, false);
                pickAxeTransform.SetGridCount(4, 350, 550);

                foreach (var pickaxe in App.Instance.Player.Blacksmith.Pickaxes)
                {
                    if (pickaxe.StaticPickaxe.Type != PickaxeType.Donate)
                        continue;

                    Instantiate(_itemPickaxePrefab, pickAxeTransform.transform, false)
                        .Initialize(pickaxe);
                }
                // rest

                FillEtc(ShopPackType.WorkShopBooster);
                FillEtc(ShopPackType.Tnt);
                FillEtc(ShopPackType.Potion);
                FillEtc(ShopPackType.Hilt);

                LayoutRebuilder.ForceRebuildLayoutImmediate(_etcScroll.content);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);

            if (!_crystalsToggle.isOn && !_salePackToggle.isOn && !_specialOfferToggle.isOn)
            {
                HideLoading();
            }
            else
            {
                if(InAppPurchaseManager.Instance.IsInitialized)
                    HideLoading();
            }

        }

        public void FillEtc(ShopPackType type)
        {
            var fillList = App.Instance.StaticData.ShopPacks.Where(x => x.Type == type);

            var headerItem = Instantiate(_etcHeaderPrefab, _etcScroll.content.transform, false);
            var containerItem = Instantiate(_etcContainerPrefab, _etcScroll.content.transform, false)
                .transform;

            headerItem.Initialize(LocalizationHelper.GetLocale(type.ToString()));

            foreach (var shopPack in fillList)
            {
                var item = Instantiate(_etcItemPrefab, containerItem, false);
                item.Initialize(shopPack);
            }
        }

        #region IAP/purchase

        private void OnBuySpecialOffer()
        {
            if (_specialOfferToggle.isOn)
            {
                OnToggle();
            }
        }

      /*  private void OnIapPurchaseComplete(IapPurchaseCompleteEvent eventData)
        {
#if UNITY_EDITOR
            OnIapValidationComplete(new ValidateIapResponse(eventData.Product, true));
#else
            var validateRequest = new ValidateIapRequest(eventData.Product.receipt);
            NetworkManager.Instance.Send<ValidateIapResponse>(validateRequest, OnIapValidationComplete, OnIapValidationError);
#endif
        }

        private void OnIapValidationComplete(ValidateIapResponse response)
        {
            App.Instance.Services.LogService.Log("Iap receipt validation complete");
            App.Instance.Player.Wallet.Add(new Currency(CurrencyType.Crystals, 0),
                IncomeSourceType.FromBuy);
        }*/

        private void OnIapValidationError(string message)
        {
            App.Instance.Services.LogService.Log("Iap receipt validation error");
        }

        #endregion

        public void Filter()
        {
            _specialOfferScroll.gameObject.SetActive(_specialOfferToggle.isOn);
            _salePackScroll.gameObject.SetActive(_salePackToggle.isOn);
            _resourceScroll.gameObject.SetActive(_resourceToggle.isOn);
            _goldScroll.gameObject.SetActive(_goldToggle.isOn);
            _crystalScroll.gameObject.SetActive(_crystalsToggle.isOn);
            _etcScroll.gameObject.SetActive(_etcToggle.isOn);

            _specialOfferEmptyContent.SetActive(false);

            _specialOfferLabel.color = _specialOfferToggle.isOn ? _activeToggleColor : _unActiveToggleColor;
            _salePackLabel.color = _salePackToggle.isOn ? _activeToggleColor : _unActiveToggleColor;
            _goldLabel.color = _goldToggle.isOn ? _activeToggleColor : _unActiveToggleColor;
            _resourceLabel.color = _resourceToggle.isOn ? _activeToggleColor : _unActiveToggleColor;
            _crystalsLabel.color = _crystalsToggle.isOn ? _activeToggleColor : _unActiveToggleColor;
            _etcLabel.color = _etcToggle.isOn ? _activeToggleColor : _unActiveToggleColor;
        }

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
          //  WindowManager.Instance.Close<WindowShopPopup>();
            Clear();

            base.OnShow(withPause, withCurrencies);

            StartCoroutine(UpdateCanvas());

          //  _isWideScreen = (float)Screen.width / Screen.height > 2;
           // _salePackScroll.content.pivot = new Vector2(_isWideScreen ? 0.5f : 0 , 0.5f);

            _goldRedDot.Show(App.Instance.Controllers.RedDotsController.NewShopPacks.Count);
            _goldToggle.gameObject.SetActive(App.Instance.Player.Dungeon.LastOpenedTier.Number > 0);
        }

      public override void OnBecameCurrent()
        {
            base.OnBecameCurrent();
          //  WindowManager.Instance.Close<WindowShopPopup>();
        }

        public override void OnClose()
        {
            base.OnClose();

            _onClose?.Invoke();
            Clear();
        }

        public void ShowLoading()
        {
            _loadingIcon.DOKill();
            _loadingIcon.alpha = 1;
        }

        public void HideLoading()
        {
            _loadingIcon.DOKill();
            _loadingIcon.DOFade(0, 1)
                .SetUpdate(true)
                .SetDelay(0.3f);
        }

        private void Update()
        {
            if (_soundTime > 0)
            {
                _soundTime -= Time.deltaTime;
                return;
            }

            _soundTime = Random.Range(3f, 8f);
            var randomSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.ShopSounds.Length);
            var randomSound = App.Instance.ReferencesTables.Sounds.ShopSounds[randomSoundIndex];
            AudioManager.Instance.PlaySound(randomSound);
        }

        private IEnumerator ScrollTo(ScrollRect scroll, int childNumber, bool immediately = false)
        {
            yield return new WaitUntil(() => IsReady);

            var target =  _etcScroll.content.GetChild(childNumber).GetComponent<RectTransform>();


            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            scroll.content.DOKill();

            var position = -target.anchoredPosition.y;
            var distance = Mathf.Abs(scroll.content.anchoredPosition.y - position);
            var duration = Mathf.Clamp01(distance / 2);

            scroll
                .content
                .DOAnchorPosY(position, immediately ? 0f : duration)
                .OnComplete(() => {  })
                .SetUpdate(true);
        }

        private IEnumerator UpdateCanvas()
        {
            yield return new WaitForEndOfFrame();

            yield return new WaitForSecondsRealtime(0.5f);
            Ready();
        }

        private void Clear()
        {
            _onClose = null;
        }

        public void Intialize(Action onClose)
        {
            OpenSalePack();
            _onClose = onClose;
        }
    }
}