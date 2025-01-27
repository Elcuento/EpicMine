using System;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using DG.Tweening;
using DragonBones;
using TMPro;
using UnityEngine;
using Chest = BlackTemple.EpicMine.Core.Chest;
using Transform = UnityEngine.Transform;

namespace BlackTemple.EpicMine
{
    public class WindowOpenChest : WindowBase
    {
        [SerializeField] private GameObject _closedPanel;
        [SerializeField] private CanvasGroup _openedPanel;

        [Space]
        [SerializeField] private GameObject _lights;

        [Space]
        [SerializeField] private GameObject _closedPanelSimpleLevel;
        [SerializeField] private GameObject _closedPanelRoyalLevel;

        [Space]
        [SerializeField] private UnityArmatureComponent _simpleChest;
        [SerializeField] private UnityArmatureComponent _royalChest;

        [Space]
        [SerializeField] private GameObject _closedPanelSimpleHeaderBackground;
        [SerializeField] private GameObject _closedPanelRoyalHeaderBackground;

        [Space]
        [SerializeField] private TextMeshProUGUI _closedPanelHeader;
        [SerializeField] private TextMeshProUGUI _closedPanelSimpleLevelText;
        [SerializeField] private TextMeshProUGUI _closedPanelRoyalLevelText;


        [Space]
        [SerializeField] private ItemView _dropItemPrefab;
        [SerializeField] private Transform _dropItemsContainer;

        private Chest _chest;
        private ChestType _chestType;
        private int _level;
        private bool _isOpened;
        private Action _onClose;
        private long? _droppedCrystals;
        private long? _droppedArtefacts;
        private Pack _dropPack;


        public void Initialize(Chest chest, Action onClose = null)
        {
            Clear();

            _chest = chest;
            _onClose = onClose;

            InitializeInternal();
        }

        public void Initialize(ChestType chestType, int chestLevel, Action onClose = null)
        {
            Clear();

            _chestType = chestType;
            _level = chestLevel;
            _onClose = onClose;

            InitializeInternal();
        }

        public void Initialize(ChestType chestType, int chestLevel, long droppedCrystals, long droppedArtefacts, Action onClose = null)
        {
            Clear();

            _chestType = chestType;
            _level = chestLevel;
            _droppedCrystals = droppedCrystals;
            _droppedArtefacts = droppedArtefacts;
            _onClose = onClose;

            InitializeInternal();
        }


        public void Open()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            if (_isOpened)
            {
                Close();
                return;
            }

            // open broken chest in village
            if (_chest != null)
            {
                _chest.Open((success, droppedCrystals, droppedArtefacts) =>
                {
                    if (!success)
                        return;

                    OnChestOpen(droppedCrystals, droppedArtefacts);
                });
            }

            // force open chest in village
            else if (_droppedCrystals != null && _droppedArtefacts != null)
                OnChestOpen(_droppedCrystals.Value, _droppedArtefacts.Value);

            // force open chest in mine
            else
            {
                var forceOpenAmount = StaticHelper.GetChestForceCompleteCost(_chestType, DateTime.MinValue);
                var forceOpenCost = new Currency(CurrencyType.Crystals, forceOpenAmount);
                if (!App.Instance.Player.Wallet.Has(forceOpenCost))
                {
                    WindowManager.Instance.Show<WindowShop>()
                        .OpenCrystals();
                    return; //s
                }


                var res = App.Instance.Player.Burglar.OpenChest(_chestType);

   
                    App.Instance.Player.Wallet.Remove(forceOpenCost);

                    OnChestOpen(res.Item3, res.Item2);

                    var openedEvent = new MineChestOpenedEvent(_chestType, _level);
                    EventManager.Instance.Publish(openedEvent);

                    var parameters = new CustomEventParameters
                    {
                        Int = new Dictionary<string, int>
                        {
                            { "type", (int)_chestType },
                            { "level", _level },
                            { "spent_crystals", (int)res.Item1},
                            { "dropped_crystals", (int)res.Item3},
                            { "dropped_artefacts",(int) res.Item2 }
                        }
                    };
                    App.Instance.Services.AnalyticsService.CustomEvent("force_open_chest", parameters);
                

            }
        }


        public override void OnClose()
        {
            base.OnClose();
            _onClose?.Invoke();
            Clear();
        }


        private void Clear()
        {
            _chest = null;
            _level = 0;
            _isOpened = false;
            _droppedCrystals = null;
            _droppedArtefacts = null;
            _onClose = null;

            _closedPanel.SetActive(true);
            _openedPanel.gameObject.SetActive(false);

            _openedPanel.DOKill();
            _openedPanel.alpha = 0;

            _closedPanelRoyalLevel.SetActive(false);
            _closedPanelSimpleLevel.SetActive(false);

            _closedPanelSimpleHeaderBackground.SetActive(false);
            _closedPanelRoyalHeaderBackground.SetActive(false);

            _simpleChest.gameObject.SetActive(false);
            _royalChest.gameObject.SetActive(false);

            _lights.SetActive(false);

            _dropItemsContainer.ClearChildObjects();
            _dropPack = null;
        }

        private void InitializeInternal()
        {
            int level;
            ChestType chestType;

            if (_chest != null)
            {
                level = _chest.Level;
                chestType = _chest.Type;
            }
            else
            {
                level = _level;
                chestType = _chestType;
            }

            var levelLocale = $"{ level + 1 } { LocalizationHelper.GetLocale("level") }";
            switch (chestType)
            {
                case ChestType.Simple:
                    _closedPanelSimpleHeaderBackground.SetActive(true);

                    _simpleChest.gameObject.SetActive(true);
                    _simpleChest.animation?.GotoAndStopByFrame("open");

                    _closedPanelHeader.text = LocalizationHelper.GetLocale("simple_chest");
                    _closedPanelSimpleLevel.gameObject.SetActive(true);
                    _closedPanelSimpleLevelText.text = levelLocale;
                    break;
                case ChestType.Royal:
                    _closedPanelRoyalHeaderBackground.SetActive(true);

                    _royalChest.gameObject.SetActive(true);
                    _royalChest.animation?.GotoAndStopByFrame("open");

                    _closedPanelHeader.text = LocalizationHelper.GetLocale("royal_chest");
                    _closedPanelRoyalLevel.gameObject.SetActive(true);
                    _closedPanelRoyalLevelText.text = levelLocale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnChestOpen(long droppedCrystals, long droppedArtefacts)
        {
            _lights.SetActive(true);

            int level;
            ChestType chestType;

            if (_chest != null)
            {
                level = _chest.Level;
                chestType = _chest.Type;
            }
            else
            {
                level = _level;
                chestType = _chestType;
            }

            _isOpened = true;
            _dropPack = StaticHelper.GetChestRandomDrop(chestType, level, droppedCrystals, droppedArtefacts);

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
            EventManager.Instance.Publish(new OpenChestEvent(chestType));

            _closedPanel.SetActive(false);
            _openedPanel.gameObject.SetActive(true);

            _openedPanel.DOFade(1, 0.1f)
                .SetDelay(0.5f)
                .SetUpdate(true);

            if (_chestType == ChestType.Royal) _royalChest.animation.Play("open", 1);
            else _simpleChest.animation.Play("open", 1);
        }
    }
}