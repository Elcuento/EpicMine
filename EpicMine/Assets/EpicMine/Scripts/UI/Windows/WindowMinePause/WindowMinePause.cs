using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Mine = BlackTemple.EpicMine.Core.Mine;
using Tier = BlackTemple.EpicMine.Core.Tier;

namespace BlackTemple.EpicMine
{
    public class WindowMinePause : WindowBase
    {
        [SerializeField] private Checkbox _music;
        [SerializeField] private Checkbox _sound;
        [SerializeField] private Checkbox _lowQuality;

        [SerializeField] private GameObject _previousMineButton;
        [SerializeField] private GameObject _nextMineButton;

        [Header("Quests")]
        [SerializeField] private RectTransform _questsIcon;
        [SerializeField] private GameObject _questsTasksButton;

        private Tier _selectedTier;
        private Mine _selectedMine;

        [Space]
        [SerializeField] private Image _language;

        public void OnClickSwitchLanguage()
        {
            WindowManager.Instance.Show<WindowSwitchLanguage>()
                .Initialize(OnCloseLanguageWindow);
        }

        private void OnCloseLanguageWindow()
        {
            _language.sprite = SpriteHelper.GetFlagLongCode(LocalizationHelper.GetCurrentLanguage().ToString());
        }


        public void GoToVillage()
        {
            MineHelper.AddEndMiningEventToAnalytics(isAlreadyCompleted: _selectedMine.IsComplete);
            MineHelper.ClearTempStorage(withClearSelectedMine: false);
            SceneManager.Instance.LoadScene(ScenesNames.Village);
            Close();
        }

        public void GoToTiers()
        {
            MineHelper.AddEndMiningEventToAnalytics(isAlreadyCompleted: _selectedMine.IsComplete);
            MineHelper.ClearTempStorage(withClearSelectedMine: false);
            SceneManager.Instance.LoadScene(ScenesNames.Tiers);
            Close();
        }

        public void GoToPreviousMine()
        {
            var mineNumber = _selectedMine.Number;
            if (mineNumber <= 0)
                return;

            mineNumber--;
            var tier = _selectedTier;
            var mine = tier.Mines[mineNumber];

            MineHelper.AddEndMiningEventToAnalytics(isAlreadyCompleted: _selectedMine.IsComplete);
            MineHelper.ClearTempStorage();

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedTier, tier);
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedMine, mine);

            SceneManager.Instance.LoadScene(ScenesNames.Mine);
            Close();
        }

        public void GoToNextMine()
        {
            if (!_selectedMine.IsComplete || _selectedMine.IsLast)
                return;

            var mineNumber = _selectedMine.Number;
            mineNumber++;
            var tier = _selectedTier;
            var mine = tier.Mines[mineNumber];

            MineHelper.AddEndMiningEventToAnalytics(isAlreadyCompleted: _selectedMine.IsComplete);
            MineHelper.ClearTempStorage();

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedTier, tier);
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedMine, mine);

            SceneManager.Instance.LoadScene(ScenesNames.Mine);
            Close();
        }

        public void Restart()
        {
            MineHelper.AddEndMiningEventToAnalytics(isAlreadyCompleted: _selectedMine.IsComplete);
            MineHelper.ClearTempStorage(withClearSelectedMine: false);
            SceneManager.Instance.LoadScene(ScenesNames.Mine);
            Close();
        }

        public void RateUs()
        {
            WindowManager.Instance.Show<WindowRateUs>();
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            _lowQuality.SetOn(PlayerPrefsHelper.LoadDefault(PlayerPrefsType.WallCracks, false));
            
            _language.sprite = SpriteHelper.GetFlagLongCode(LocalizationHelper.GetCurrentLanguage().ToString());
            _music.SetOn(!AudioManager.Instance.IsMusicMuted, immediately: true);
            _sound.SetOn(!AudioManager.Instance.IsSoundsMuted, immediately: true);

            _music.OnChange += OnMusicChange;
            _sound.OnChange += OnSoundChange;
            _lowQuality.OnChange += OnQualityChange;

            _selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            _selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            _previousMineButton.SetActive(_selectedMine.Number > 0);

            _nextMineButton.SetActive(_selectedMine.IsComplete && _selectedMine.Number < LocalConfigs.TierMinesCount - 1);

            _questsTasksButton.SetActive(App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks));

            var isVillageQuestAvailable = QuestHelper.IsVillageHasQuests();

            _questsIcon.gameObject.SetActive(isVillageQuestAvailable);

            if (isVillageQuestAvailable)
            {
                _questsIcon.DOKill();
                _questsIcon.DOAnchorPosX(_questsIcon.transform.localPosition.x - 40, 2)
                    .SetEase(Ease.InSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetUpdate(true);
            }
        }

        public void OnClickTasksQuests()
        {
            WindowManager.Instance.Show<WindowDailyTasksQuest>()
                .OpenQuests();
        }


        public override void OnClose()
        {
            base.OnClose();
            _music.OnChange -= OnMusicChange;
            _sound.OnChange -= OnSoundChange;
            _lowQuality.OnChange -= OnQualityChange;
        }

        private void OnQualityChange(bool isOn)
        {
            PlayerPrefsHelper.Save(PlayerPrefsType.WallCracks, isOn);
            EventManager.Instance.Publish(new ChangeSettingsQualityEvent(isOn));
        }

        private void OnSoundChange(bool isOn)
        {
            AudioManager.Instance.SetSoundsMuted(!isOn);
        }

        private void OnMusicChange(bool isOn)
        {
            AudioManager.Instance.SetMusicMuted(!isOn);
        }
    }
}