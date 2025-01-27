using System;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class WindowOpenEnchantedChest : WindowBase
    {
        [SerializeField] private GameObject _closedPanel;
        [SerializeField] private GameObject _openedPanel;

        [Space]
        [SerializeField] private GameObject _closedPanelAmberChest;
        [SerializeField] private GameObject _closedPanelRubyChest;
        [SerializeField] private GameObject _closedPanelLazuriteChest;
        [SerializeField] private GameObject _closedPanelMalachiteChest;

        [Space]
        [SerializeField] private Color _amberHeaderBackgroundColor;
        [SerializeField] private Color _rubyHeaderBackgroundColor;
        [SerializeField] private Color _lazuriteHeaderBackgroundColor;
        [SerializeField] private Color _malachiteHeaderBackgroundColor;
        [SerializeField] private Image[] _headerBackground;

        [Space]
        [SerializeField] private Color _amberSubHeaderBackgroundColor;
        [SerializeField] private Color _rubySubHeaderBackgroundColor;
        [SerializeField] private Color _lazuriteSubHeaderBackgroundColor;
        [SerializeField] private Color _malachiteSubHeaderBackgroundColor;
        [SerializeField] private Image _subHeaderBackground;

        [Space]
        [SerializeField] private Color _amberSubHeaderTextColor;
        [SerializeField] private Color _rubySubHeaderTextColor;
        [SerializeField] private Color _lazuriteSubHeaderTextColor;
        [SerializeField] private Color _malachiteSubHeaderTextColor;

        [Space]
        [SerializeField] private TextMeshProUGUI _closedPanelHeaderText;
        [SerializeField] private TextMeshProUGUI _closedPanelSubHeaderText;

        [Space]
        [SerializeField] private GameObject _openedPanelAmberChest;
        [SerializeField] private GameObject _openedPanelRubyChest;
        [SerializeField] private GameObject _openedPanelLazuriteChest;
        [SerializeField] private GameObject _openedPanelMalachiteChest;

        [Space]
        [SerializeField] private ItemView _dropItemPrefab;
        [SerializeField] private Transform _dropItemsContainer;

        [Space]
        [SerializeField] private TextMeshProUGUI _openByCrystalsPriceText;
        [SerializeField] private GameObject _closeButton;

        private EnchantedChestType _chestType;
        private int _level;
        private bool _isOpened;
        private Action _onClose;
        private int? _droppedCrystals;
        private int? _droppedArtefacts;
        private Pack _dropPack;

        public void Initialize(EnchantedChestType chestType, int level, Action onClose = null)
        {
            Clear();

            _chestType = chestType;
            _level = level;
            _onClose = onClose;

            InitializeInternal();

            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnAdRewardedVideoCompleted;


            _closeButton.SetActive(true);
        }


        int GetRandomDroppedArtefactsAmount(Configs configsData)
        {

            var maxArtefactsCount = configsData.Dungeon.TierOpenArtefactsCost;

            maxArtefactsCount = maxArtefactsCount - App.Instance.Player.Artefacts.Amount;

            var artefactsMin = configsData.EnchantedChests.Drop.Min;
            var artefactsMax = configsData.EnchantedChests.Drop.Max;

            if (artefactsMin > maxArtefactsCount)
                artefactsMin = maxArtefactsCount;

            if (artefactsMax > maxArtefactsCount)
                artefactsMax = maxArtefactsCount;

            var randomAmount = Random.Range((int)artefactsMin, (int)artefactsMax);
            return randomAmount;

        }

        public void OpenByCrystals()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            int openCrystalsAmount;
            switch (_chestType)
            {
                case EnchantedChestType.Amber:
                    openCrystalsAmount = App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Amber].Price;
                    break;
                case EnchantedChestType.Ruby:
                    openCrystalsAmount = App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Ruby].Price;
                    break;
                case EnchantedChestType.Lazurite:
                    openCrystalsAmount = App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Lazurite].Price;
                    break;
                case EnchantedChestType.Malachite:
                    openCrystalsAmount = App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Malachite].Price;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var openCost = new Currency(CurrencyType.Crystals, openCrystalsAmount);
            if (!App.Instance.Player.Wallet.Has(openCost))
            {
                WindowManager.Instance.Show<WindowShop>()
                    .OpenCrystals();

                return;
            }

            var staticData = App.Instance.StaticData;

            var chestPriceInCrystals = 8;

            if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, chestPriceInCrystals))
            {
                Debug.LogError("Not enough money");
                return ;
            }

            var droppedArtifacts = GetRandomDroppedArtefactsAmount(staticData.Configs);

            App.Instance.Player.Artefacts.Add(droppedArtifacts);

            OnChestOpen(droppedArtifacts);

            var parameters = new CustomEventParameters
            {
                Int = new Dictionary<string, int>
                {
                    { "type", (int)_chestType },
                    { "level", _level },
                    { "spent_crystals", chestPriceInCrystals },
                    { "dropped_artefacts", droppedArtifacts }
                }
            };
            App.Instance.Services.AnalyticsService.CustomEvent("open_enchanted_chest_by_crystals", parameters);
        }

        public void OpenByAd()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

#if UNITY_EDITOR
            OnAdRewardedVideoCompleted(AdSource.OpenEnchantedChest,true, "", 0);
#else
            App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.OpenEnchantedChest);
#endif
        }


        public override void OnClose()
        {
            base.OnClose();
            _onClose?.Invoke();
            Clear();
        }


        private void OnAdRewardedVideoCompleted(AdSource adSource, bool isShowed, string rewardId, int rewardAmount)
        {
            if (!isShowed)
                return;

            if (adSource != AdSource.OpenEnchantedChest)
                return;

            SendOpenByAdRequest();
        }

        private void SendOpenByAdRequest()
        {
            var staticData = App.Instance.StaticData;

            var droppedArtifacts = GetRandomDroppedArtefactsAmount(staticData.Configs);

            App.Instance.Player.Artefacts.Add(droppedArtifacts);

            OnChestOpen(droppedArtifacts);

            var parameters = new CustomEventParameters
            {
                Int = new Dictionary<string, int>
                {
                    { "type", (int)_chestType },
                    { "level", _level },
                    { "dropped_artefacts", droppedArtifacts }
                }
            };
            App.Instance.Services.AnalyticsService.CustomEvent("open_enchanted_chest_by_ad", parameters);
        }



        private void Clear()
        {
            _isOpened = false;
            _droppedCrystals = null;
            _droppedArtefacts = null;
            _onClose = null;

            _closedPanel.SetActive(true);
            _openedPanel.SetActive(false);

            _closedPanelAmberChest.SetActive(false);
            _closedPanelRubyChest.SetActive(false);
            _closedPanelLazuriteChest.SetActive(false);
            _closedPanelMalachiteChest.SetActive(false);

            _openedPanelAmberChest.SetActive(false);
            _openedPanelRubyChest.SetActive(false);
            _openedPanelLazuriteChest.SetActive(false);
            _openedPanelMalachiteChest.SetActive(false);

            foreach (var headerBackgroundImage in _headerBackground)
                headerBackgroundImage.color = Color.white;
            
            _closedPanelHeaderText.text = string.Empty;

            _subHeaderBackground.color = Color.white;
            _closedPanelSubHeaderText.text = string.Empty;
            _closedPanelSubHeaderText.color = Color.white;
            _closeButton.SetActive(false);

            transform.DOKill();

            _dropItemsContainer.ClearChildObjects();
            _dropPack = null;

            if (App.Instance != null)
                App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnAdRewardedVideoCompleted;
        }

        private void InitializeInternal()
        {
            Color headerBackgroundColor;
            Color subHeaderBackgroundColor;
            Color subHeaderTextColor;
            string subHeaderLocaleKey;
            string price;

            switch (_chestType)
            {
                case EnchantedChestType.Amber:
                    _closedPanelAmberChest.SetActive(true);
                    _openedPanelAmberChest.SetActive(true);
                    subHeaderLocaleKey = "enchanted_chest_amber";
                    headerBackgroundColor = _amberHeaderBackgroundColor;
                    subHeaderBackgroundColor = _amberSubHeaderBackgroundColor;
                    subHeaderTextColor = _amberSubHeaderTextColor;
                    price = App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Amber].Price.ToString();
                    break;
                case EnchantedChestType.Ruby:
                    _closedPanelRubyChest.SetActive(true);
                    _openedPanelRubyChest.SetActive(true);
                    subHeaderLocaleKey = "enchanted_chest_ruby";
                    headerBackgroundColor = _rubyHeaderBackgroundColor;
                    subHeaderBackgroundColor = _rubySubHeaderBackgroundColor;
                    subHeaderTextColor = _rubySubHeaderTextColor;
                    price = App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Ruby].Price.ToString();
                    break;
                case EnchantedChestType.Lazurite:
                    _closedPanelLazuriteChest.SetActive(true);
                    _openedPanelLazuriteChest.SetActive(true);
                    subHeaderLocaleKey = "enchanted_chest_lazurite";
                    headerBackgroundColor = _lazuriteHeaderBackgroundColor;
                    subHeaderBackgroundColor = _lazuriteSubHeaderBackgroundColor;
                    subHeaderTextColor = _lazuriteSubHeaderTextColor;
                    price = App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Lazurite].Price.ToString();
                    break;
                case EnchantedChestType.Malachite:
                    _closedPanelMalachiteChest.SetActive(true);
                    _openedPanelMalachiteChest.SetActive(true);
                    subHeaderLocaleKey = "enchanted_chest_malachite";
                    headerBackgroundColor = _malachiteHeaderBackgroundColor;
                    subHeaderBackgroundColor = _malachiteSubHeaderBackgroundColor;
                    subHeaderTextColor = _malachiteSubHeaderTextColor;
                    price = App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Malachite].Price.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var subHeader = LocalizationHelper.GetLocale(subHeaderLocaleKey);

            _closedPanelHeaderText.text = LocalizationHelper.GetLocale("enchanted_chest");
            _closedPanelSubHeaderText.text = subHeader;

            foreach (var headerBackgroundImage in _headerBackground)
                headerBackgroundImage.color = headerBackgroundColor;

            _subHeaderBackground.color = subHeaderBackgroundColor;
            _closedPanelSubHeaderText.color = subHeaderTextColor;

            _openByCrystalsPriceText.text = price;
        }

        private void OnChestOpen(int droppedArtefacts)
        {
            _isOpened = true;
            _dropPack = StaticHelper.GetEnchantedChestRandomDrop(_chestType, _level, droppedArtefacts);

            App.Instance.Player.Inventory.Add(_dropPack, IncomeSourceType.FromMineChest);
            App.Instance.Player.Wallet.Add(_dropPack, IncomeSourceType.FromMineChest);
            App.Instance.Player.Artefacts.Add(_dropPack.Artefacts);

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.OpenChest);

            foreach (var item in _dropPack.Items)
            {
                var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                itemView.Initialize(item);
            }

            foreach (var currency in _dropPack.Currencies)
            {
                var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                itemView.Initialize(currency);
            }

            if (_dropPack.Artefacts > 0)
            {
                var artefactsView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                var artefactsSprite = App.Instance.ReferencesTables.Sprites.ArtefactIcon;
                artefactsView.Initialize(artefactsSprite, _dropPack.Artefacts);
            }

            _closedPanel.SetActive(false);
            _openedPanel.SetActive(true);
        }
    }
}