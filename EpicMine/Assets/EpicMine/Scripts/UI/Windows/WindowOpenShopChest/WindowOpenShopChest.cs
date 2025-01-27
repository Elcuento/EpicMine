using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class WindowOpenShopChest : WindowBase
    {
        [SerializeField] private GameObject _closedPanel;
        [SerializeField] private GameObject _openedPanel;

        [Space]
        [SerializeField] private GameObject _closedPanelMinerChest;
        [SerializeField] private GameObject _closedPanelSorcererChest;
        [SerializeField] private GameObject _closedPanelGreatnessChest;

        [Space]
        [SerializeField] private GameObject _closedPanelSimpleHeaderBackground;
        [SerializeField] private GameObject _closedPanelRoyalHeaderBackground;

        [Space]
        [SerializeField] private TextMeshProUGUI _closedPanelHeader;

        [Space]
        [SerializeField] private GameObject _openedPanelMinerChest;
        [SerializeField] private GameObject _openedPanelSorcererChest;
        [SerializeField] private GameObject _openedPanelGreatnessChest;

        [Space]
        [SerializeField] private ItemView _dropItemPrefab;
        [SerializeField] private Transform _dropItemsContainer;

        private ShopChest _shopChest;
        private bool _isOpened;
        private Action _onClose;


        public void Initialize(ShopChest chest, Action onClose = null)
        {
            Clear();
            
            _shopChest = chest;
            _onClose = onClose;
            
            _closedPanelSimpleHeaderBackground.SetActive(true);
            _closedPanelHeader.text = LocalizationHelper.GetLocale(chest.Id);

            switch (_shopChest.Type)
            {
                case ShopChestType.Miner:
                    _closedPanelMinerChest.SetActive(true);
                    _openedPanelMinerChest.SetActive(true);
                    break;
                case ShopChestType.Sorcerer:
                    _closedPanelSorcererChest.SetActive(true);
                    _openedPanelSorcererChest.SetActive(true);
                    break;
                case ShopChestType.Greatness:
                    _closedPanelGreatnessChest.SetActive(true);
                    _openedPanelGreatnessChest.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Open()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            if (_isOpened)
            {
                Close();
                return;
            }
            
            if (!App.Instance.Player.Inventory.Remove(new Item(_shopChest.Id, 1), SpendType.Using))
            {
                Close();
                return;
            }

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.OpenChest);

            _isOpened = true;
            _closedPanel.SetActive(false);
            _openedPanel.SetActive(true);

            var hilts = new List<CommonDLL.Dto.Item>();

            if (_shopChest.Hilt1Rarity != null)
            {
                var randomValue = Random.Range(0, 100f);
                if (randomValue <= _shopChest.Hilt1Chance)
                {
                    var availableHilts = StaticHelper.GetHiltsByPickaxeRare(_shopChest.Hilt1Rarity.Value).Where(h => h.DropCategory > 0).ToList();
                    if (availableHilts.Count > 0)
                    {
                        var randomIndex = Random.Range(0, availableHilts.Count);
                        var randomHilt = availableHilts[randomIndex];
                        hilts.Add(new Item(randomHilt.Id, 1));
                    }
                }

                var logMessage = "Shop chest: {0}, hilt 1 random value: {1}";
                App.Instance.Services.LogService.Log(string.Format(logMessage, _shopChest.Id, randomValue));
            }

            if (_shopChest.Hilt2Rarity != null)
            {
                var randomValue = Random.Range(0, 100f);
                if (randomValue <= _shopChest.Hilt2Chance)
                {
                    var availableHilts = StaticHelper.GetHiltsByPickaxeRare(_shopChest.Hilt2Rarity.Value).Where(h => h.DropCategory > 0).ToList();
                    if (availableHilts.Count > 0)
                    {
                        var randomIndex = Random.Range(0, availableHilts.Count);
                        var randomHilt = availableHilts[randomIndex];
                        hilts.Add(new CommonDLL.Dto.Item(randomHilt.Id, 1));
                    }
                }

                var logMessage = "Shop chest: {0}, hilt 2 random value: {1}";
                App.Instance.Services.LogService.Log(string.Format(logMessage, _shopChest.Id, randomValue));
            }

            if (_shopChest.Hilt3Rarity != null)
            {
                var randomValue = Random.Range(0, 100f);
                if (randomValue <= _shopChest.Hilt3Chance)
                {
                    var availableHilts = StaticHelper.GetHiltsByPickaxeRare(_shopChest.Hilt3Rarity.Value).Where(h => h.DropCategory > 0).ToList();
                    if (availableHilts.Count > 0)
                    {
                        var randomIndex = Random.Range(0, availableHilts.Count);
                        var randomHilt = availableHilts[randomIndex];
                        hilts.Add(new Item(randomHilt.Id, 1));
                    }
                }

                var logMessage = "Shop chest: {0}, hilt 3 random value: {1}";
                App.Instance.Services.LogService.Log(string.Format(logMessage, _shopChest.Id, randomValue));
            }

            foreach (var item in hilts)
            {
                var itemView = Instantiate(_dropItemPrefab, _dropItemsContainer, false);
                itemView.Initialize(item);
                App.Instance.Player.Inventory.Add(new Item(item.Id, 1), IncomeSourceType.FromShopChest);
            }

            var parameters = new CustomEventParameters { String = new Dictionary<string, string> { { "id", _shopChest.Id } } };
            App.Instance.Services.AnalyticsService.CustomEvent("open_shop_chest", parameters);
        }


        public override void OnClose()
        {
            base.OnClose();
            _onClose?.Invoke();
            Clear();
        }


        private void Clear()
        {
            _isOpened = false;

            _closedPanel.SetActive(true);
            _openedPanel.SetActive(false);
            
            _closedPanelMinerChest.SetActive(false);
            _closedPanelSorcererChest.SetActive(false);
            _closedPanelGreatnessChest.SetActive(false);

            _closedPanelSimpleHeaderBackground.SetActive(false);
            _closedPanelRoyalHeaderBackground.SetActive(false);

            _openedPanelMinerChest.SetActive(false);
            _openedPanelSorcererChest.SetActive(false);
            _openedPanelGreatnessChest.SetActive(false);

            _dropItemsContainer.ClearChildObjects();
        }
    }
}