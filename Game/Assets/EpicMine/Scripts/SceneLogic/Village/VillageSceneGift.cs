using System;
using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class VillageSceneGift : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _simpleGiftIcons;
        [SerializeField] private List<GameObject> _royalGiftIcons;

        [SerializeField] private GameObject _worldGift;
        [SerializeField] private GameObject _uiGift;
        [SerializeField] private TextMeshProUGUI _timer;

        public void OpenWindow()
        {
            WindowManager.Instance.Show<WindowGift>();
        }

        private void Awake()
        {
            Initialize();
            EventManager.Instance.Subscribe<GiftOpenEvent>(OnGiftOpen);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<GiftOpenEvent>(OnGiftOpen);
        }

        private void Initialize()
        {
            Clear();

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier))
                return;

            if (!App.Instance.Player.Gifts.IsGiftExists)
                return;

            ShowGiftIcons();

            if (App.Instance.Player.Gifts.IsCanOpen)
            {
                _worldGift.SetActive(true);
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.GiftReady);
            }
            else
            {
                _uiGift.SetActive(true);
                StartCoroutine(UpdateTimer());
            }
        }

        private void ShowGiftIcons()
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

        private void OnGiftOpen(GiftOpenEvent eventData)
        {
            Initialize();
        }

        private void Clear()
        {
            _worldGift.SetActive(false);
            _uiGift.SetActive(false);

            foreach (var simpleGiftIcon in _simpleGiftIcons)
                simpleGiftIcon.SetActive(false);

            foreach (var royalGiftIcon in _royalGiftIcons)
                royalGiftIcon.SetActive(false);

            StopAllCoroutines();
        }
    }
}