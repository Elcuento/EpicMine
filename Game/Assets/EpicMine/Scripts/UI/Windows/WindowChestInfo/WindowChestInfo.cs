using System;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowChestInfo : WindowBase
    {
        [SerializeField] private GameObject _simpleIcon;
        [SerializeField] private GameObject _royalIcon;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Space]
        [SerializeField] private GameObject _startPanel;
        [SerializeField] private GameObject _breakingPanel;
        [SerializeField] private GameObject _anotherBreakingPanel;

        [Space]
        [SerializeField] private GameObject _time;
        [SerializeField] private TextMeshProUGUI _timeText;

        [Space]
        [SerializeField] private GameObject _timeLeft;
        [SerializeField] private TextMeshProUGUI _timeLeftText;

        [Space]
        public TextMeshProUGUI BreakingPanelOpenNowCost;
        [SerializeField] private TextMeshProUGUI _breakingPanelSpeedUp;

        [Space]
        [SerializeField] private TextMeshProUGUI _anotherBreakingPanelOpenNowCost;

        private Core.Chest _chest;


        public void Initialize(Core.Chest chest)
        {
            Clear();

            _chest = chest;
            UpdateCommonInfo();
            UpdateState();
        }


        public void StartBreaking()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _chest.StartBreaking(OnStartBreakingComplete);
        }

        public void ForceComplete(bool isTutorial = false)
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            if (!isTutorial)
            {
                var forceOpenAmount = StaticHelper.GetChestForceCompleteCost(_chest.Type, _chest.StartBreakingTime);
                var forceOpenCost = new Currency(CurrencyType.Crystals, forceOpenAmount);

                if (!App.Instance.Player.Wallet.Has(forceOpenCost))
                {
                    WindowManager.Instance.Show<WindowShop>()
                        .OpenCrystals();

                    return;
                }

                WindowManager
                    .Instance
                    .Show<WindowCurrencySpendConfirm>()
                    .Initialize(
                        forceOpenCost,
                        () => { _chest.ForceOpen(OnForceOpenComplete); },
                        "window_currency_spend_confirm_description_chest",
                        "window_currency_spend_confirm_ok_chest");
            }
            else
            {    _chest.ForceOpen(OnForceOpenComplete, true);}
        }

        private  void OnForceOpenComplete(bool success, long spentCrystals, long droppedCrystals, long droppedArtefacts)
        {

            var forceOpenCost = new Currency(CurrencyType.Crystals, spentCrystals);
            App.Instance.Player.Wallet.Remove(forceOpenCost);

            Close();
            var windowOpenChest = WindowManager.Instance.Show<WindowOpenChest>();
            windowOpenChest.Initialize(_chest.Type, _chest.Level, droppedCrystals, droppedArtefacts);
        }

        public void SpeedUp()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _chest.SpeedUp();
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            Unsubscribe();
            Subscribe();
        }

        public override void OnClose()
        {
            base.OnClose();
            Clear();
            Unsubscribe();
        }


        private void OnDestroy()
        {
            Unsubscribe();
        }


        private void Clear()
        {
            _time.SetActive(false);
            _timeLeft.SetActive(false);

            _startPanel.SetActive(false);
            _breakingPanel.SetActive(false);
            _anotherBreakingPanel.SetActive(false);
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<ChestBreakingTimeLeftEvent>(OnChestBreakingTimeLeft);
            EventManager.Instance.Subscribe<ChestBreakedEvent>(OnChestBreaked);
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<ChestBreakingTimeLeftEvent>(OnChestBreakingTimeLeft);
            EventManager.Instance.Unsubscribe<ChestBreakedEvent>(OnChestBreaked);
        }


        private void UpdateCommonInfo()
        {
            _simpleIcon.SetActive(false);
            _royalIcon.SetActive(false);

            _levelText.text = (_chest.Level + 1).ToString();
            switch (_chest.Type)
            {
                case ChestType.Simple:
                    _simpleIcon.SetActive(true);
                    _timeText.text = $"{StaticHelper.GetChestConfigs(_chest.Type).BreakingTimeInMinutes / 60}{LocalizationHelper.GetLocale("hours")}.";
                    _titleText.text = LocalizationHelper.GetLocale("simple_chest");
                    break;
                case ChestType.Royal:
                    _royalIcon.SetActive(true);
                    _timeText.text = $"{StaticHelper.GetChestConfigs(_chest.Type).BreakingTimeInMinutes / 60}{LocalizationHelper.GetLocale("hours")}.";
                    _titleText.text = LocalizationHelper.GetLocale("royal_chest");
                    break;
            }
        }

        private void UpdateState()
        {
            Clear();

            if (!_chest.IsBreakingStarted)
            {
                foreach (var chest in App.Instance.Player.Burglar.Chests)
                {
                    if (chest.IsBreakingStarted)
                    { 
                        ShowAnotherBreakingPanel();
                        return;
                    }
                }

                ShowStartPanel();
                return;
            }

            ShowBreakingPanel();
        }
       

        private void ShowStartPanel()
        {
            _startPanel.SetActive(true);
            _time.SetActive(true);
        }

        private void ShowBreakingPanel()
        {
            _breakingPanel.SetActive(true);
            _timeLeft.SetActive(true);

            _timeLeftText.text = TimeHelper.Format(StaticHelper.GetChestBreakingTimeLeft(_chest.Type, _chest.StartBreakingTime.Value));
            BreakingPanelOpenNowCost.text = StaticHelper.GetChestForceCompleteCost(_chest.Type, _chest.StartBreakingTime.Value).ToString();

            var speedUpLocale = LocalizationHelper.GetLocale("window_chest_info_speed_up");
            _breakingPanelSpeedUp.text = string.Format(speedUpLocale, App.Instance.StaticData.Configs.Burglar.ChestBreakingSpeedUpMinutes);
        }

        private void ShowAnotherBreakingPanel()
        {
            _anotherBreakingPanel.SetActive(true);
            _time.SetActive(true);

            _anotherBreakingPanelOpenNowCost.text = StaticHelper.GetChestForceCompleteCost(_chest.Type, DateTime.MinValue).ToString();
        }


        private void OnStartBreakingComplete(bool success)
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.StartBreakingChest);
            UpdateState();
        }

        private void OnChestBreakingTimeLeft(ChestBreakingTimeLeftEvent eventData)
        {
            if (_chest == eventData.Chest)
            {
                _timeLeftText.text = TimeHelper.Format(eventData.TimeLeft);
                BreakingPanelOpenNowCost.text = StaticHelper.GetChestForceCompleteCost(_chest.Type,  _chest.StartBreakingTime.Value).ToString();
            }
        }

        private void OnChestBreaked(ChestBreakedEvent eventData)
        {
            if (eventData.Chest == _chest)
            {
                Close();
                return;
            }

            UpdateState();
        }
    }
}