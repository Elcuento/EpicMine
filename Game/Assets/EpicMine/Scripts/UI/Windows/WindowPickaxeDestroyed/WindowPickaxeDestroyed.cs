using System;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Currency = BlackTemple.EpicMine.Dto.Currency;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class WindowPickaxeDestroyed : WindowBase
    {
        [SerializeField] private ItemView _itemPrefab;
        [SerializeField] private Sprite _timerFullSprite;
        [SerializeField] private Sprite[] _timerSprites;
        [SerializeField] private TextMeshProUGUI _emptyItemsHint;

        [Space]
        [SerializeField] private GameObject _windowStats;
        [SerializeField] private Transform _windowStatsItemsContainer;

        [SerializeField] private GameObject _ContinueCrystalButton;
        [SerializeField] private GameObject _ContinueAddButton;
        [SerializeField] private GameObject _ContinueNoThanksButton;
        [SerializeField] private TextMeshProUGUI _ContinueCrystalButtonValue;

        [SerializeField] private GameObject _windowContinue;

        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private RatingView _scoreRating;
        [SerializeField] private TextMeshProUGUI _recordText;
        [SerializeField] private RatingView _recordRating;
        [SerializeField] private Image _timer;

        [Header("Buttons")]
        [SerializeField] private GameObject _buttons;

        [Header("Buttons/Quests")]
        [SerializeField] private RectTransform _questsIcon;
        [SerializeField] private GameObject _questsTasksButton;

        private bool _isContinueAvailable;
        private bool _isWaitingAdWatching;
        private bool _isWaitingCrystalSpend;
        private bool _isShopOpened;

        private float _continueTimeout;
        private Action _onRestart;
        private Action _onContinue;

        private readonly string[] _emptyItemsHintsLocalizationKeys = {"window_pickaxe_destroyed_hint_1", "window_pickaxe_destroyed_hint_2", "window_pickaxe_destroyed_hint_3"};

        private void Start()
        {
            EventManager.Instance.Subscribe<WindowCloseEvent>(OnWindowClose);
            EventManager.Instance.Subscribe<WindowOpenEvent>(OnWindowOpen);

            CheckOpenedWindows();
        }

        private void CheckOpenedWindows()
        {
            if (WindowManager.Instance.IsOpen<WindowShopOffer>() || WindowManager.Instance.IsOpen<WindowShop>())
                _isShopOpened = true;
        }

        private void OnWindowOpen(WindowOpenEvent data)
        {
            if(data.Window is WindowShopOffer || data.Window is WindowShop || data.Window is WindowForceAds)

            _isShopOpened = true;
        }


        private void OnWindowClose(WindowCloseEvent data)
        {
            if (WindowManager.Instance.IsOpen<WindowShopOffer>() || WindowManager.Instance.IsOpen<WindowShop>() || WindowManager.Instance.IsOpen<WindowForceAds>())
                return;

            _isShopOpened = false;
        }

        public void Initialize(int score, Core.Mine blMine, Action onRestart = null, Action onContinue = null,
            bool isContinueAvailable = false)
        {
            _isContinueAvailable = isContinueAvailable;

            if (_isContinueAvailable)
                ShowWindowContinue();
            else
            {
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeDestroyedWindow);
                HideWindowContinue();
            }

            _scoreText.text = score.ToString();
            _recordText.text = blMine.Highscore.ToString();

            var isHardcore = App.Instance
                .Services
                .RuntimeStorage
                .Load<bool>(RuntimeStorageKeys.IsHardcoreMode);

            var ratingType = isHardcore ? ViewRatingType.Skulls : ViewRatingType.Stars;
            var rating = RatingHelper.GetMineHardcoreRating(score);
            _scoreRating.Initialize(rating, ratingType);
            _recordRating.Initialize(isHardcore ? blMine.HardcoreRating : blMine.Rating, ratingType);

            _onRestart = onRestart;
            _onContinue = onContinue;

            if (App.Instance.Services.AdvertisementService.IsForceAdsAvailable())
            {
                _isWaitingAdWatching = true;
                App.Instance.Services.AdvertisementService.ShowForceAds();
            }
            
        }

        public void GoToVillage()
        {
            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            MineHelper.AddEndMiningEventToAnalytics(isAlreadyCompleted: selectedMine.IsComplete);

            MineHelper.ClearTempStorage(withClearSelectedMine: false);
            SceneManager.Instance.LoadScene(ScenesNames.Village);
            Close();
        }

        public void GoToTiers()
        {
            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            MineHelper.AddEndMiningEventToAnalytics(isAlreadyCompleted: selectedMine.IsComplete);

            MineHelper.ClearTempStorage(withClearSelectedMine: false);
            SceneManager.Instance.LoadScene(ScenesNames.Tiers);
            Close();
        }

        public void Restart()
        {
            _onRestart?.Invoke();

            MineHelper.ClearTempStorage(withClearSelectedMine: false);
            Close();
        }

        public void Continue()
        {
            _isWaitingAdWatching = true;
            App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.RefillPickaxeHealth);
        }

        public void Stop()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeDestroyedWindow);
            HideWindowContinue();
        }

        public void HideWindowContinue()
        {
            _isContinueAvailable = false;
            _windowStats.SetActive(true);
            _windowContinue.SetActive(false);
            _buttons.SetActive(true);

            var dailyTaskShowed =
                App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks);

            _questsTasksButton.SetActive(dailyTaskShowed);

            var isVillageQuestAvailable = QuestHelper.IsVillageHasQuests();

            _questsIcon.gameObject.SetActive(isVillageQuestAvailable);

            if (isVillageQuestAvailable)
            {
                _questsIcon.DOKill();
                _questsIcon.DOAnchorPosX(_questsIcon.transform.localPosition.x - 40, 2)
                    .SetEase(Ease.InSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void OnClickQuestsTasks()
        {
            WindowManager.Instance.Show<WindowDailyTasksQuest>()
                .OpenLastTab();
        }

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnAdRewardedVideoCompleted;
            App.Instance.Services.AdvertisementService.OnInterstitialCompleted += OnInterstitialCompleted;

            var lastMineDroppedItems = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<string, int>>(RuntimeStorageKeys.LastMineDroppedItems);

            var lastMineDroppedCurrencies = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<CurrencyType, int>>(RuntimeStorageKeys.LastMineDroppedCurrencies);

            if (lastMineDroppedItems != null)
            {
                foreach (var droppedItem in lastMineDroppedItems)
                {
                    var item = Instantiate(_itemPrefab, _windowStatsItemsContainer, false);
                    var dtoItem = new Item(droppedItem.Key, droppedItem.Value);
                    item.Initialize(dtoItem);
                }
            }

            if (lastMineDroppedCurrencies != null)
            {
                foreach (var droppedCurrency in lastMineDroppedCurrencies)
                {
                    var item = Instantiate(_itemPrefab, _windowStatsItemsContainer, false);
                    var dtoCurrency = new Dto.Currency(droppedCurrency.Key, droppedCurrency.Value);
                    item.Initialize(dtoCurrency);
                }
            }

            if ((lastMineDroppedItems == null || lastMineDroppedItems.Count <= 0) && (lastMineDroppedCurrencies == null || lastMineDroppedCurrencies.Count <= 0))
            {
                var randomHintKeyIndex = Random.Range(0, _emptyItemsHintsLocalizationKeys.Length);
                var randomHintKey = _emptyItemsHintsLocalizationKeys[randomHintKeyIndex];
                _emptyItemsHint.text = LocalizationHelper.GetLocale(randomHintKey);
            }
            else
                _emptyItemsHint.text = string.Empty;
        }

        public override void OnClose()
        {
            base.OnClose();

            foreach (Transform item in _windowStatsItemsContainer)
                Destroy(item.gameObject);

            _emptyItemsHint.text = string.Empty;

            Unsubscribe();
        }

        public void OnClickPayContinuePayCrystal()
        {

            var currency = new Currency(CurrencyType.Crystals, App.Instance.StaticData.Configs.Dungeon.ContinueCrystalPrice);

            if (App.Instance.Player.Wallet.Has(currency))
            {
                _isWaitingCrystalSpend = true;

                var priceInCrystals = App.Instance.StaticData.Configs.Dungeon.ContinueCrystalPrice;

                if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, priceInCrystals))
                {
                    Debug.LogError("Not enough crystals");
                    return;
                }

                OnPayCrystal(priceInCrystals);

                _isWaitingCrystalSpend = false;
            }
            else
            {
                _isShopOpened = true;

                WindowManager.Instance.Show<WindowShop>(true,true)
                    .OpenCrystals();
            }
        }

        private void OnPayCrystal(long cost)
        {
            _onContinue?.Invoke();
            Close();
        }


        private void OnInterstitialCompleted()
        {
            _isWaitingAdWatching = false;
            WindowManager.Instance.Show<WindowForceAds>();
        }


        private void OnAdRewardedVideoCompleted(AdSource adSource, bool isShowed, string rewardId, int rewardAmount)
        {
            if (!isShowed)
            {
                _isWaitingAdWatching = false;
                return;
            }

            if (adSource == AdSource.RefillPickaxeHealth)
            {
                _onContinue?.Invoke();
                Close();
            }
        }

        private void ShowWindowContinue()
        {

            _buttons.SetActive(false);
            _windowStats.SetActive(false);
            _windowContinue.SetActive(true);
            _timer.sprite = _timerFullSprite;

            _isWaitingAdWatching = false;
            _isWaitingCrystalSpend = false;
            _continueTimeout = App.Instance.StaticData.Configs.Windows.PickaxeDestroyed.ContinueAvailableTime;

            var healthRefillCount = App.Instance
                .Services
                .RuntimeStorage
                .Load<int>(RuntimeStorageKeys.HealthRefillCount);

            _ContinueNoThanksButton.gameObject.SetActive(false);
            _ContinueAddButton.gameObject.SetActive(healthRefillCount < 2);
            _ContinueCrystalButtonValue.text =
                $"{App.Instance.StaticData.Configs.Dungeon.ContinueCrystalPrice}";

            TimeManager.Instance.UnscaledTweenDelay(2, LateShowContinue);
        }

        public void LateShowContinue()
        {
            _ContinueNoThanksButton.gameObject.SetActive(true);
        }

        private void Unsubscribe()
        {
            if (App.Instance == null || EventManager.Instance == null) return;

            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnAdRewardedVideoCompleted;
            App.Instance.Services.AdvertisementService.OnInterstitialCompleted -= OnInterstitialCompleted;

            EventManager.Instance.Unsubscribe<WindowCloseEvent>(OnWindowClose);
            EventManager.Instance.Unsubscribe<WindowOpenEvent>(OnWindowOpen);
        }


        private void Update()
        {
            if (!_isContinueAvailable || _isWaitingAdWatching || _isWaitingCrystalSpend || _isShopOpened)
                return;

            if (_continueTimeout <= 0)
            {
                _isContinueAvailable = false;
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeDestroyedWindow);
                HideWindowContinue();
                return;
            }

            _continueTimeout -= Time.unscaledDeltaTime;
            var oneSpriteTime = App.Instance.StaticData.Configs.Windows.PickaxeDestroyed.ContinueAvailableTime / _timerSprites.Length;

            var spriteNumber = Mathf.CeilToInt(_continueTimeout / oneSpriteTime);
            if (spriteNumber > _timerSprites.Length - 1)
                return;

            _timer.sprite = _timerSprites[spriteNumber];
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}