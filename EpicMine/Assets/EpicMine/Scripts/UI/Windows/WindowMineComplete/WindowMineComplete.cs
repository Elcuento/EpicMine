using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowMineComplete : WindowBase
    {
        [SerializeField] private string _animationName;
        [SerializeField] private ItemView _itemPrefab;

        [Space]
        [SerializeField] private Animator _pickaxesAnimator;
        [SerializeField] private Transform _droppedItemsContainer;
        [SerializeField] private GameObject _againButton;

        [Header("Quests")]
        [SerializeField] private RectTransform _questsIcon;
        [SerializeField] private GameObject _questsTasksButton;

        [SerializeField] private GameObject _taskHint;

        public void GoToVillage()
        {
            Close();
            MineHelper.ClearTempStorage();
            SceneManager.Instance.LoadScene(ScenesNames.Village);
        }

        public void GoToTiers()
        {
            Close();
            MineHelper.ClearTempStorage();
            SceneManager.Instance.LoadScene(ScenesNames.Tiers);
        }

        public void ToMineAgain()
        {

            SceneManager.Instance.LoadScene(ScenesNames.Mine);

            MineHelper.ClearTempStorage(withClearSelectedMine: false);
            Close();


        }

        public void OnClickQuestsTasks()
        {
            WindowManager.Instance.Show<WindowDailyTasksQuest>()
                .OpenLastTab();
        }

        public void GoToNextMine()
        {
            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            var mineNumber = selectedMine.Number;
            var tierNumber = selectedTier.Number;

            mineNumber++;
            if (mineNumber >= LocalConfigs.TierMinesCount)
            {
                tierNumber++;
                mineNumber = 0;
            }
            

            var energy = App.Instance.Services.RuntimeStorage.Load<int>(RuntimeStorageKeys.Energy);
            var tier = App.Instance.Player.Dungeon.Tiers[tierNumber];
            var mine = tier.Mines[mineNumber];

            MineHelper.ClearTempStorage();
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.Energy, energy);

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedTier, tier);
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedMine, mine);
            
            SceneManager.Instance.LoadScene(ScenesNames.Mine);
            Close();
        }

        private void UnSubscribe()
        {
            if (EventManager.Instance == null)
                return;

            App.Instance.Services.AdvertisementService.OnInterstitialCompleted -= OnInterstitialCompleted;
        }


        private void OnInterstitialCompleted()
        {
            WindowManager.Instance.Show<WindowForceAds>();
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            App.Instance.Services.AdvertisementService.OnInterstitialCompleted += OnInterstitialCompleted;

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.MineComplete);
            _pickaxesAnimator.Play(_animationName);

            var lastMineDroppedItems = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<string, int>>(RuntimeStorageKeys.LastMineDroppedItems);

            if (lastMineDroppedItems != null)
            {
                foreach (var droppedItem in lastMineDroppedItems)
                {
                    var item = Instantiate(_itemPrefab, _droppedItemsContainer, false);
                    var dtoItem = new Item(droppedItem.Key, droppedItem.Value);
                    item.Initialize(dtoItem);
                }
            }

            var lastMineDroppedCurrencies = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<CurrencyType, int>>(RuntimeStorageKeys.LastMineDroppedCurrencies);

            if (lastMineDroppedCurrencies != null)
            {
                foreach (var droppedCurrency in lastMineDroppedCurrencies)
                {
                    var item = Instantiate(_itemPrefab, _droppedItemsContainer, false);
                    var dtoCurrency = new Dto.Currency(droppedCurrency.Key, droppedCurrency.Value);
                    item.Initialize(dtoCurrency);
                }
            }

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (!selectedMine.IsLast)
                MineHelper.AddEndMiningEventToAnalytics(true);

            var dailyTaskShowed =
                App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks);

             _againButton.SetActive(dailyTaskShowed);
             _questsTasksButton.SetActive(dailyTaskShowed);

            var isVillageQuestAvailable = QuestHelper.IsVillageHasQuests();

            _questsIcon.gameObject.SetActive(isVillageQuestAvailable);

            if (isVillageQuestAvailable)
            {
                _questsIcon.DOAnchorPosX(_questsIcon.transform.localPosition.x - 40, 2)
                    .SetEase(Ease.InSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            if (!App.Instance.Controllers.TutorialController.IsComplete)
            {
                if (selectedMine.Number == 3)
                    App.Instance.Services.AdvertisementService.ShowForceAds(false);
            }
            else App.Instance.Services.AdvertisementService.ShowForceAds();

            //Debug.Log(App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks));
            if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks))
            {
                var completedQuest = App.Instance.Player.Quests.QuestList.Find(x => x.Status == QuestStatusType.Started && x.IsReady);
                if (completedQuest!=null && !PlayerPrefsHelper.LoadDefault(PlayerPrefsType.TutorialHintShowMineTasksButton, false))
                {
                    _taskHint.gameObject.SetActive(true);
                }

            }
        }

        public void OnClickCloseHint()
        {
            _taskHint.gameObject.SetActive(false);
            PlayerPrefsHelper.Save(PlayerPrefsType.TutorialHintShowMineTasksButton, true);
            WindowManager.Instance.Show<WindowDailyTasksQuest>()
                .OpenQuests();
        }

        public override void OnClose()
        {
            base.OnClose();

            foreach (Transform item in _droppedItemsContainer)
                Destroy(item.gameObject);

            UnSubscribe();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }
    }
}