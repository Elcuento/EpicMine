using System;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class VillageScenePlaceForChest : MonoBehaviour
    {
        public TextMeshProUGUI ForceOpenCost;

        [SerializeField] private GameObject _simpleIcon;
        [SerializeField] private GameObject _royalIcon;
        [SerializeField] private GameObject _empty;
        [SerializeField] private GameObject _locked;
        [SerializeField] private TextMeshProUGUI _lockedText;
        [SerializeField] private GameObject _breaking;
        [SerializeField] private TextMeshProUGUI _breakingText;
        [SerializeField] private GameObject _completeArrow;

        public Core.Chest Chest { get; private set; }
        

        public void Initialize(Core.Chest chest)
        {
            Clear();

            _empty.SetActive(false);

            Chest = chest;
            switch (chest.Type)
            {
                case ChestType.Simple:
                    _simpleIcon.SetActive(true);
                    break;
                case ChestType.Royal:
                    _royalIcon.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Chest.IsBreakingStarted)
            {
                if (Chest.IsBroken)
                    SetBroken();
                else
                    SetBreaking();
            }
            else
                SetLocked();
        }

        public void Click()
        {
            if (Chest == null)
            {
                var window = WindowManager.Instance.Show<WindowInformation>();
                window.Initialize("window_place_for_chest_header", "window_place_for_chest_description", "window_place_for_chest_button");
                return;
            }

            if (Chest.IsBroken)
            {
                WindowManager.Instance.Show<WindowOpenChest>().Initialize(Chest);
                return;
            }

            var windowChestInfo = WindowManager.Instance.Show<WindowChestInfo>();
            windowChestInfo.Initialize(Chest);
        }

        public void SetLocked()
        {
            _breaking.SetActive(false);
            _locked.SetActive(true);
            _completeArrow.SetActive(false);
            _lockedText.text = $"{ StaticHelper.GetChestConfigs(Chest.Type).BreakingTimeInMinutes / 60 }{ LocalizationHelper.GetLocale("hours") }";
        }

        public void SetBreaking()
        {
            _locked.SetActive(false);
            _breaking.SetActive(true);
            _completeArrow.SetActive(false);

            var timeLeft = StaticHelper.GetChestBreakingTimeLeft(Chest.Type, Chest.StartBreakingTime.Value);
            SetBreakingTimeLeft(timeLeft);
        }

        public void SetBreakingTimeLeft(TimeSpan timeLeft)
        {
            _breakingText.text = TimeHelper.Format(timeLeft);
            ForceOpenCost.text = StaticHelper.GetChestForceCompleteCost(Chest.Type, Chest.StartBreakingTime).ToString();
        }

        public void SetBroken()
        {
            _locked.SetActive(false);
            _breaking.SetActive(false);
            _completeArrow.SetActive(true);
        }

        public void Clear()
        {
            Chest = null;
            _empty.SetActive(true);
            _locked.SetActive(false);
            _breaking.SetActive(false);
            _simpleIcon.SetActive(false);
            _royalIcon.SetActive(false);
            _completeArrow.SetActive(false);
        }
    }
}