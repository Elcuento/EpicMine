using System;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowLastMineComplete : WindowBase
    {
        [SerializeField] private string _animationName;


        [SerializeField] private GameObject _againButton;

        [SerializeField] private Animator _pickaxesAnimator;
        [SerializeField] private TextMeshProUGUI _artefactsAmount;
        [SerializeField] private TextMeshProUGUI _hintPopupArtefactsAmount;
        [SerializeField] private GameObject _hintPopup;

        [Space]
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private TextMeshProUGUI _buttonArtefactsCost;


        [Header("Quests")]
        [SerializeField] private RectTransform _questsIcon;
        [SerializeField] private GameObject _questsTasksButton;

        private Action _onUnlockTier;
        private Core.Tier _nextTier;


        private void UnSubscribe()
        {
            if (EventManager.Instance == null)
                return;

            App.Instance.Services.AdvertisementService.OnInterstitialCompleted -= OnInterstitialCompleted;
        }


        public void Initialize(Action onUnlockTier)
        {
            _onUnlockTier = onUnlockTier;

            var isVillageQuestAvailable = QuestHelper.IsVillageHasQuests();

            _questsIcon.gameObject.SetActive(isVillageQuestAvailable);

            if(isVillageQuestAvailable)
            {
                _questsIcon.DOAnchorPosX(_questsIcon.transform.localPosition.x - 40, 2)
                    .SetEase(Ease.InSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            var dailyTaskShowed =
                App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks);

            _questsTasksButton.SetActive(dailyTaskShowed);
            _againButton.SetActive(dailyTaskShowed);
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        public override void OnClose()
        {
            base.OnClose();
            UnSubscribe();
        }

        public void OnClickQuestsTasks()
        {
            WindowManager.Instance.Show<WindowDailyTasksQuest>()
                .OpenLastTab();
        }

        public void ToMineAgain()
        {
            SceneManager.Instance.LoadScene(ScenesNames.Mine);

            MineHelper.ClearTempStorage(withClearSelectedMine: false);
            Close();
        }

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

        public void OpenNextTier()
        {
            if (_nextTier != null)
            {
                if (App.Instance.Player.Artefacts.Amount >= _nextTier.StaticTier.RequireArtefacts)
                {
                    AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
                    _nextTier.Open(OnTierOpenComplete);
                    return;
                }
            }
            
            Close();
            MineHelper.ClearTempStorage(false);
            SceneManager.Instance.LoadScene(ScenesNames.Tiers);
        }

        private void OnTierOpenComplete(bool success)
        {
            if (success)
            {
                Close();

                _nextTier.Open();
                _onUnlockTier?.Invoke();
            }
        }

        public void ShowHintPopup()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _hintPopup.SetActive(true);
        }

        public void CloseHintPopup()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _hintPopup.SetActive(false);
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

            var needArtifactsAmount = 0;
            _nextTier = App.Instance.Player.Dungeon.Tiers.FirstOrDefault(t => t.IsOpen == false);

            if (_nextTier != null)
                needArtifactsAmount = _nextTier.StaticTier.RequireArtefacts;

            _buttonText.text = LocalizationHelper.GetLocale(App.Instance.Player.Artefacts.Amount >= needArtifactsAmount
                ? "window_last_mine_complete_open_new_tier_button"
                : "window_last_mine_complete_mines_list_button");

            _buttonArtefactsCost.text = needArtifactsAmount.ToString();

            var artifactsAmount = $"{App.Instance.Player.Artefacts.Amount}/{LocalConfigs.MaxArtefacts}";
            _artefactsAmount.text = _hintPopupArtefactsAmount.text = artifactsAmount;

            MineHelper.AddEndMiningEventToAnalytics(true);

            App.Instance.Services.AdvertisementService.ShowForceAds();
        }
    }
}