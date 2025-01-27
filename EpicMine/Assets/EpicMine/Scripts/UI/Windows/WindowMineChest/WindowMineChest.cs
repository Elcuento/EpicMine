using System;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowMineChest : WindowBase
    {
        [SerializeField] private GameObject _simpleIcon;
        [SerializeField] private GameObject _royalIcon;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _chestsCountText;
        [SerializeField] private TextMeshProUGUI _openCostText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Image _sendToVillageButton;

        private int _level;
        private ChestType _chestType;
        private Action _onClose;


        public void Initialize(int level, ChestType chestType, Action onClose)
        {
            Clear();

            _level = level;
            _chestType = chestType;
            _onClose = onClose;

            var maxChestCount = LocalConfigs.MaxChestCount;

            _levelText.text = (level + 1).ToString();
            _chestsCountText.text = $"{App.Instance.Player.Burglar.Chests.Count}/{maxChestCount}";
            _openCostText.text = StaticHelper.GetChestForceCompleteCost(_chestType, DateTime.MinValue).ToString();

            var canSendChest = App.Instance.Player.Burglar.Chests.Count < maxChestCount;
            _sendToVillageButton.sprite = canSendChest
                ? App.Instance.ReferencesTables.Sprites.ButtonGrown
                : App.Instance.ReferencesTables.Sprites.ButtonGrey;

            _timeText.text = $"{StaticHelper.GetChestConfigs(chestType).BreakingTimeInMinutes / 60}{LocalizationHelper.GetLocale("hours")}.";

            switch (chestType)
            {
                case ChestType.Simple:
                    _simpleIcon.SetActive(true);
                    _titleText.text = LocalizationHelper.GetLocale("simple_chest");
                    break;
                case ChestType.Royal:
                    _royalIcon.SetActive(true);
                    _titleText.text = LocalizationHelper.GetLocale("royal_chest");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("chestType", chestType, null);
            }
        }


        public void ForceOpen()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            var forceOpenAmount = StaticHelper.GetChestForceCompleteCost(_chestType, DateTime.MinValue);
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
                    () => {
                        if (!App.Instance.Player.Wallet.Has(forceOpenCost))
                            return;

                        var window = WindowManager.Instance.Show<WindowOpenChest>();
                        window.Initialize(_chestType, _level, _onClose);
                        _onClose = null;
                        Close();
                    },
                    "window_currency_spend_confirm_description_chest",
                    "window_currency_spend_confirm_ok_chest");
        }

        public void SendToVillage()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            App.Instance.Player.Burglar.AddChest(_chestType, _level, added =>
            {
                if (added)
                    Close();
            });
        }


        public override void OnClose()
        {
            base.OnClose();
            _onClose?.Invoke();
            Clear();
        }


        private void Clear()
        {
            _simpleIcon.SetActive(false);
            _royalIcon.SetActive(false);
        }
    }
}