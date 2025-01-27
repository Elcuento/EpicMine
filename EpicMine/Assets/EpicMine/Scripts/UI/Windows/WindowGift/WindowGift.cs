using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowGift : WindowBase
    {
        [SerializeField] private GameObject _closedPanel;
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private List<GameObject> _checkboxes;

        [SerializeField] private List<GameObject> _simpleGiftIcons;
        [SerializeField] private List<GameObject> _royalGiftIcons;
        [SerializeField] private TextMeshProUGUI _progressTitle;
        [SerializeField] private TextMeshProUGUI _timer;


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
            Initialize();
        }

        public override void OnClose()
        {
            base.OnClose();
            Clear();
        }


        public void Open()
        {
            WindowManager
                .Instance
                .Show<WindowOpenGift>()
                .Initialize(App.Instance.Player.Gifts.OpenedCount);

            Close();
        }


        private void Initialize()
        {
            Clear();
            ShowInnerPanel();
            ShowProgress();
        }

        private void ShowInnerPanel()
        {
            if (App.Instance.Player.Gifts.OpenedCount < App.Instance.StaticData.Configs.Gifts.DailyCount - 1)
            {
                foreach (var simpleGiftIcon in _simpleGiftIcons)
                    simpleGiftIcon.SetActive(true);
            }
            else
            {
                foreach (var royalGiftIcon in _royalGiftIcons)
                    royalGiftIcon.SetActive(true);
            }

            if (App.Instance.Player.Gifts.IsCanOpen)
                _readyPanel.SetActive(true);
            else
            {
                _closedPanel.SetActive(true);
                StartCoroutine(UpdateTimer());
            }
        }

        private void ShowProgress()
        {
            var progressLocale = LocalizationHelper.GetLocale("window_gift_progress_title");
            _progressTitle.text = string.Format(
                progressLocale,
                App.Instance.Player.Gifts.OpenedCount,
                App.Instance.StaticData.Configs.Gifts.DailyCount);

            for (var i = 0; i < _checkboxes.Count; i++)
            {
                var checkbox = _checkboxes[i];
                checkbox.SetActive(App.Instance.Player.Gifts.OpenedCount > i);
            }
        }

        private IEnumerator UpdateTimer()
        {
            var timeLeft = App.Instance.Player.Gifts.ReadyTime;
            var oneSecond = new WaitForSecondsRealtime(1f);

            while (timeLeft > 0)
            {
                timeLeft = App.Instance.Player.Gifts.ReadyTime;
                var time = TimeHelper.Format(App.Instance.Player.Gifts.ReadyTime);
                _timer.text = time; //$"{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";
                yield return oneSecond;
            }

            Initialize();
        }

        private void Clear()
        {
            _readyPanel.SetActive(false);
            _closedPanel.SetActive(false);

            foreach (var checkbox in _checkboxes)
                checkbox.SetActive(false);

            foreach (var simpleGiftIcon in _simpleGiftIcons)
                simpleGiftIcon.SetActive(false);

            foreach (var royalGiftIcon in _royalGiftIcons)
                royalGiftIcon.SetActive(false);

            StopAllCoroutines();
        }
    }
}