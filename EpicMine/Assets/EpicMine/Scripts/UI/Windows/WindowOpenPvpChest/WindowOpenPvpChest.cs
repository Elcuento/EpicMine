using System;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using DG.Tweening;
using DragonBones;
using TMPro;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;
using Transform = UnityEngine.Transform;

namespace BlackTemple.EpicMine
{
    public class WindowOpenPvpChest : WindowBase
    {
        private Action _onClose;

        [SerializeField] private GameObject _closedPanel;
        [SerializeField] private CanvasGroup _openedPanel;

        [Space] [SerializeField] private UnityArmatureComponent _simpleChest;
        [SerializeField] private UnityArmatureComponent _royalChest;
        [SerializeField] private UnityArmatureComponent _winnerChest;

        [SerializeField] private GameObject _lights;

        [Space] [SerializeField] private GameObject _doubleBonusCrystalButton;
        [SerializeField] private TextMeshProUGUI _doubleBonusCrystalButtonLabel;
        [SerializeField] private GameObject _doubleBonusAdButton;
        [SerializeField] private GameObject _takeDoubleBonusButton;
        [SerializeField] private GameObject _takeBonusButton;

        [SerializeField] private TextMeshProUGUI _getItemText;

        [Space] [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private TextMeshProUGUI _smallHeaderText;

        [Space] [SerializeField] private ItemView _dropItemPrefab;
        [SerializeField] private Transform _dropItemsContainer;

        [Space] private Pack _dropPack;

        [Space] private PvpChestType _chestType;
        private int _level;
        private bool _isOpened;
        private bool _isDoubled;
        private bool _isGetReward;
        private bool _isInventoryChest;

        private void OnChestOpen(int droppedArtefacts)
        {
            _isOpened = true;
            _dropPack = StaticHelper.GetPvpChestRandomDrop(_chestType, _level, droppedArtefacts);

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.OpenChest);


            foreach (var currency in _dropPack.Currencies)
            {
                if (currency.Amount <= 0) continue;

                var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                itemView.Initialize(currency);
            }

            foreach (var item in _dropPack.Items)
            {
                if (item.Amount <= 0) continue;

                var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                itemView.Initialize(item);
            }

            _closedPanel.SetActive(false);
            _openedPanel.gameObject.SetActive(true);
            _openedPanel.DOFade(1, 0.1f)
                .SetDelay(0.5f)
                .SetUpdate(true);

            if (_chestType == PvpChestType.Simple)
                _simpleChest.animation.Play("open", 1);
            if (_chestType == PvpChestType.Royal)
                _royalChest.animation.Play("open", 1);
            if (_chestType == PvpChestType.Winner)
                _winnerChest.animation.Play("open", 1);

            _lights.SetActive(true);

            ShowGet();
        }

        public void Initialize(PvpChestType chestType, int level, bool isInventoryChest, Action onClose = null)
        {
            Clear();

            _chestType = chestType;
            _level = level;
            _isInventoryChest = isInventoryChest;
            _onClose = onClose;

            _simpleChest.gameObject.SetActive(_chestType == PvpChestType.Simple);
            _royalChest.gameObject.SetActive(_chestType == PvpChestType.Royal);
            _winnerChest.gameObject.SetActive(_chestType == PvpChestType.Winner);

            _simpleChest.animation?.GotoAndStopByFrame("open", 0);
            _royalChest.animation?.GotoAndStopByFrame("open", 0);
            _winnerChest.animation?.GotoAndStopByFrame("open", 0);

            _headerText.text = LocalizationHelper.GetLocale("pvp_chest_" + chestType);
            _smallHeaderText.text = $"{LocalizationHelper.GetLocale("level")} {level + 1}";
            _doubleBonusCrystalButtonLabel.text =
                App.Instance.StaticData.Configs.Pvp.ChestDoubleBonusCrystalsCost.ToString();

            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnRewardShowed;

        }

        public void ShowGet()
        {
            _takeBonusButton.gameObject.SetActive(true);
        }

        private void Clear()
        {
            _isOpened = false;
            _onClose = null;
            _isDoubled = false;
            _isGetReward = false;
            _isInventoryChest = false;
            _getItemText.text = LocalizationHelper.GetLocale("window_workshop_collect_button");

            _lights.SetActive(false);
            _closedPanel.SetActive(true);
            _openedPanel.gameObject.SetActive(false);
            _openedPanel.DOKill();
            _openedPanel.alpha = 0;

            _doubleBonusAdButton.SetActive(true);
            _doubleBonusCrystalButton.SetActive(true);
            _takeBonusButton.gameObject.SetActive(false);
            _takeDoubleBonusButton.gameObject.SetActive(false);

            _dropItemsContainer.ClearChildObjects();
            _dropPack = null;

            if (App.Instance != null)
                App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnRewardShowed;


        }



        public void OnClickOpen()
        {
            if (_isInventoryChest && !App.Instance.Player.Inventory.Has(new Item(
                    _chestType == PvpChestType.Simple ? PvpLocalConfig.PvpSimpleChestItemId :
                    _chestType == PvpChestType.Royal ? PvpLocalConfig.PvpRoyalChestItemId :
                    PvpLocalConfig.PvpWinnerChestItemId, 1)))
            {
                Close();
                return;
            }

            if (_isOpened)
                return;

            SendOpenChest();
        }

        public void OnClickGetDoubleBonusByCrystal()
        {
            var currency = new Currency(CurrencyType.Crystals,
                App.Instance.StaticData.Configs.Pvp.ChestDoubleBonusCrystalsCost);

            if (!App.Instance.Player.Wallet.Has(currency))
            {
                WindowManager.Instance.Show<WindowShop>()
                    .OpenCrystals();
                return;
            }

            var staticData = App.Instance.StaticData;

            var priceInCrystals = staticData.Configs.Pvp.ChestDoubleBonusCrystalsCost;
            if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, priceInCrystals))
            {
                Debug.LogError("Not enough crystals");
                return;
            }

            _doubleBonusAdButton.gameObject.SetActive(false);
            _doubleBonusCrystalButton.gameObject.SetActive(false);
            _takeDoubleBonusButton.gameObject.SetActive(true);
            _takeBonusButton.gameObject.SetActive(false);

            Multiplay();

            var parameters = new CustomEventParameters
            {
                Int = new Dictionary<string, int>
                {
                    { "type", (int)_chestType },
                    { "level", _level },
                }
            };
            App.Instance.Services.AnalyticsService.CustomEvent("pvp_chest_multiply_by_crystals", parameters);

        }

        public void OnClickGetDoubleBonusByAd()
        {

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.MultiplyPvpChestReward);
        }

        public override void OnClose()
        {
            base.OnClose();
            _onClose?.Invoke();
            Clear();
        }

        public void OnClickGetReward()
        {
            if (_isGetReward) return;

            _isGetReward = true;

            App.Instance.Player.Inventory.Add(_dropPack, IncomeSourceType.FromPvp);
            App.Instance.Player.Wallet.Add(_dropPack, IncomeSourceType.FromPvp);

            App.Instance.Player.Inventory.Remove(
                new Item(
                    _chestType == PvpChestType.Simple ? PvpLocalConfig.PvpSimpleChestItemId :
                    _chestType == PvpChestType.Royal ? PvpLocalConfig.PvpRoyalChestItemId :
                    PvpLocalConfig.PvpWinnerChestItemId, 1), SpendType.Using);

            Close();
        }

        public void OnRewardShowed(AdSource adSource, bool isShowed, string rewardId, int rewardAmount)
        {
            if (!isShowed)
                return;

            if (adSource != AdSource.MultiplyPvpChestReward)
                return;

            if (_isDoubled) return;

            _isDoubled = true;

            _doubleBonusAdButton.gameObject.SetActive(false);
            _doubleBonusCrystalButton.gameObject.SetActive(false);
            _takeBonusButton.gameObject.SetActive(false);
            _takeDoubleBonusButton.gameObject.SetActive(true);
            CancelInvoke("LateShowGet");

            _getItemText.text = LocalizationHelper.GetLocale("window_workshop_collect_doubling_button");

            var parameters = new CustomEventParameters
            {
                Int = new Dictionary<string, int>
                {
                    { "type", (int)_chestType },
                    { "level", _level },
                }
            };
            App.Instance.Services.AnalyticsService.CustomEvent("pvp_chest_multiply_by_ad", parameters);

            Multiplay();

        }

        public void Multiplay()
        {
            _dropItemsContainer.ClearChildObjects();

            _dropPack.Multiplay(2);

            foreach (var currency in _dropPack.Currencies)
            {
                if (currency.Amount == 0) continue;
                var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                itemView.Initialize(currency, Color.green);

            }

            foreach (var item in _dropPack.Items)
            {
                if (item.Amount == 0) continue;
                var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                itemView.Initialize(item, Color.green);
            }
        }



        private void SendOpenChest()
        {
            if (_isInventoryChest)
            {
                OnChestOpen(0);
                return;
            }

            if (_chestType == PvpChestType.Winner)
            {
                if (App.Instance.Player.Pvp.Chests < 5)
                    return;

                App.Instance.Player.Pvp.SetChests(0);

                OnChestOpen(0);

            }
        }
    }
}
