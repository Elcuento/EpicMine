using System;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class RateUsController
    {
        public bool IsAvailableToShow { get; private set; }

        private const string WindowShowedCountPrefsKey = "RATE_US_SHOWED_COUNT";

        private const string WindowLastShowedDate = "RATE_US_LAST_SHOWED_DATE";

        private const int MaxCount = 3;

        private int _deathCount;

        private int _foundChestsCount;

        private int _showedCount;

        private DateTime _lastShowedDate;


        public RateUsController()
        {
            _showedCount = PlayerPrefs.GetInt(WindowShowedCountPrefsKey, 0);
            _lastShowedDate = PlayerPrefs.HasKey(WindowLastShowedDate) 
                ? DateTime.Parse(PlayerPrefs.GetString(WindowLastShowedDate)) 
                : DateTime.MinValue;

            if (_showedCount <= MaxCount && _lastShowedDate.AddDays(1) < DateTime.UtcNow.Date)
            {
                EventManager.Instance.Subscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeDestroy);
                EventManager.Instance.Subscribe<MineCompleteEvent>(OnMineComplete);
                EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
                EventManager.Instance.Subscribe<MineEnterEvent>(OnMineEnter);
                EventManager.Instance.Subscribe<WindowOpenEvent>(OnWindowOpen);
                SceneManager.Instance.OnSceneChange += OnSceneChange;
            }
        }

        public void Save()
        {
            PlayerPrefs.SetInt(WindowShowedCountPrefsKey, _showedCount);
            PlayerPrefs.SetString(WindowLastShowedDate, _lastShowedDate.ToShortDateString());
            PlayerPrefs.Save();
        }

        public void Clear()
        {
            IsAvailableToShow = false;
            _deathCount = 0;
            _foundChestsCount = 0;
            _showedCount = 0;
            _lastShowedDate = DateTime.MinValue;

            PlayerPrefs.DeleteKey(WindowShowedCountPrefsKey);
            PlayerPrefs.DeleteKey(WindowLastShowedDate);
            PlayerPrefs.Save();
        }


        private void OnWindowOpen(WindowOpenEvent data)
        {
            if (data.Window is WindowRateUs)
            {
                IsAvailableToShow = false;

                _showedCount++;
                _lastShowedDate = DateTime.UtcNow.Date;

                Save();

                if (EventManager.Instance != null)
                {
                    EventManager.Instance.Unsubscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeDestroy);
                    EventManager.Instance.Unsubscribe<MineCompleteEvent>(OnMineComplete);
                    EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
                    EventManager.Instance.Unsubscribe<MineEnterEvent>(OnMineEnter);
                    EventManager.Instance.Unsubscribe<WindowOpenEvent>(OnWindowOpen);
                }

                if (SceneManager.Instance != null)
                {
                    SceneManager.Instance.OnSceneChange -= OnSceneChange;
                }
            }
        }

        private void OnPickaxeDestroy(MineScenePickaxeDestroyedEvent eventData)
        {
            if (IsAvailableToShow)
                return;

            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            if (App.Instance.Player.Dungeon.LastOpenedTier != selectedTier)
                return;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (!selectedMine.IsLast)
                return;

            _deathCount++;
        }

        private void OnMineComplete(MineCompleteEvent eventData)
        {
            if (_deathCount < 3)
                return;

            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            if (App.Instance.Player.Dungeon.LastOpenedTier != selectedTier)
                return;

            if (!eventData.Mine.IsLast)
                return;

            IsAvailableToShow = true;
            _deathCount = 0;
        }


        private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            if (IsAvailableToShow)
                return;

            if (!(eventData.Section is MineSceneChestSection))
                return;

            _foundChestsCount++;
            if (_foundChestsCount < 2)
                return;

            IsAvailableToShow = true;
            _foundChestsCount = 0;
        }

        private void OnMineEnter(MineEnterEvent eventData)
        {
            _foundChestsCount = 0;
        }

        private void OnSceneChange(string from, string to)
        {
            if (!IsAvailableToShow)
                return;

            if (!App.Instance.Controllers.TutorialController.IsComplete)
                return;

            if (to == ScenesNames.Village || to == ScenesNames.Tiers)
                WindowManager.Instance.Show<WindowRateUs>();
        }
    }
}