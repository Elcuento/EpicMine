using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiItemsPanel : MonoBehaviour
    {
        public const string WorkshopBubbleHintKey = "MineWorkshopBubbleHint";

        [SerializeField] private MineSceneHero _hero;
        [SerializeField] private MineSceneUi _ui;

        [Header("WorkShopButton")]
        [SerializeField] private GameObject _workshopButton;
        [SerializeField] private GameObject _workshopBubbleHint;

        [Space]
        [SerializeField] private RectTransform _innerPanel;
        [SerializeField] private Image _background;
        [SerializeField] private Transform _buyButton;
        [SerializeField] private MineSceneUiItemsPanelItem _itemPrefab;
        [SerializeField] private RectTransform _itemsContainer;
        [SerializeField] private RectTransform _arrowIcon;
        [SerializeField] private TextMeshProUGUI _title;

        private readonly List<MineSceneUiItemsPanelItem> _items = new List<MineSceneUiItemsPanelItem>();
        private MineSceneUiItemsPanelItem _selectedItem;
        private const float ItemWidth = 160;
        private const float ItemSpace = -8;
        private const float SlideTime = 0.3f;

        private bool _isOpened;

       
        public void Toggle()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            if (!_isOpened && _items.Count <= 0)
                return;


            _innerPanel.DOKill();
            _isOpened = !_isOpened;
            TimeManager.Instance.SetPause(_isOpened);

            if (_selectedItem == null)
                _selectedItem = _items.FirstOrDefault();

            _arrowIcon.DOScaleX(_isOpened ? -1 : 1, 0f);
            _background.enabled = _isOpened;

            _title.text = _isOpened
                ? _selectedItem == null ? string.Empty : LocalizationHelper.GetLocale(_selectedItem.ItemStaticId)
                : string.Empty;
            //
            if (!_isOpened && _selectedItem != null)
                _selectedItem.transform.SetAsFirstSibling();

            var width = _isOpened ? (_items.Count + 1) * ItemWidth + (_items.Count - 1) * ItemSpace : ItemWidth;
            var size = new Vector2(width, _innerPanel.sizeDelta.y);

            _innerPanel.DOSizeDelta(size, SlideTime)
                .SetUpdate(true);

            foreach (var item in _items)
                item.UpdateState(_isOpened, _isOpened && item == _selectedItem);
        }


        public void OnBuyButtonClick()
        {
            var windowShop = WindowManager.Instance.Show<WindowShop>(withPause: true, withCurrencies: true);
            windowShop.OpenTnt();

            if (_isOpened)
                Toggle();
        }

        public void OnClickWorkShop()
        {
            WindowManager.Instance.Show<WindowWorkshop>(withPause:true);
        }

        private void Awake()
        {
            EventManager.Instance.Subscribe<InventoryNewItemAddEvent>(OnItemAdd);
            EventManager.Instance.Subscribe<InventoryItemChangeEvent>(OnItemChange);
            EventManager.Instance.Subscribe<InventoryItemRemoveExistEvent>(OnItemRemove);


            CheckWorkshopButton();
        }



        private void CheckWorkshopButton()
        {
            var tier = App.Instance.Player.Dungeon.LastOpenedTier;
            var show = false;

            if (tier != null)
            {
                if (tier.Number > 1)
                {
                    show = true;
               
                }else if (tier.Number == 1)
                {
                    var mine = tier.Mines.FindLast(x => x.IsComplete);
                    if (mine != null && mine.Number > 0)
                    {
                        show = true;
                    }
                }
            }

            if (show)
            {
                _workshopButton.SetActive(true);
                var hint = PlayerPrefs.GetInt(WorkshopBubbleHintKey, 0);
                _workshopBubbleHint.SetActive(hint == 0);
            }
           
        }

        public void OnClickWorkshopHint()
        {
            _workshopBubbleHint.SetActive(false);

            PlayerPrefs.SetInt(WorkshopBubbleHintKey, 1);
            PlayerPrefs.Save();
        }

        private void Start()
        {
            foreach (var inventoryItem in App.Instance.Player.Inventory.Items)
                UpdateItem(inventoryItem.Id, inventoryItem.Amount);

            var lastUsedItem = App.Instance.Services.RuntimeStorage.Load<string>(RuntimeStorageKeys.LastUsedItem);

            if (!string.IsNullOrEmpty(lastUsedItem))
            {
                foreach (var item in _items)
                {
                    if (item.ItemStaticId != lastUsedItem)
                        continue;

                    _selectedItem = item;
                    item.transform.SetAsFirstSibling();
                    break;
                }
            }
            else
                _selectedItem = _items.FirstOrDefault();

        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<InventoryNewItemAddEvent>(OnItemAdd);
            EventManager.Instance.Unsubscribe<InventoryItemChangeEvent>(OnItemChange);
            EventManager.Instance.Unsubscribe<InventoryItemRemoveExistEvent>(OnItemRemove);
        }


        private void OnItemAdd(InventoryNewItemAddEvent eventData)
        {
            UpdateItem(eventData.Item.Id, eventData.Item.Amount);
        }

        private void OnItemChange(InventoryItemChangeEvent eventData)
        {
            UpdateItem(eventData.Item.Id, eventData.Item.Amount);
        }

        private void OnItemRemove(InventoryItemRemoveExistEvent existEventData)
        {
            UpdateItem(existEventData.ItemId, 0);
        }


        private void UpdateItem(string id, int amount)
        {
            foreach (var item in _items)
            {
                if (item.ItemStaticId != id)
                    continue;

                if (amount <= 0)
                {
                    _items.Remove(item);
                    Destroy(item.gameObject);
                }
                else
                    item.UpdateAmount(amount);

                return;
            }

            var isTnt = StaticHelper.IsTypeOf(id, typeof(Tnt));
            var isPotion = StaticHelper.IsTypeOf(id, typeof(Potion));

            if (isTnt || isPotion)
            {
                var item = Instantiate(_itemPrefab, _itemsContainer, false);
                item.Initialize(new Item(id, amount), OnItemClick);
                _items.Add(item);
            }

            _buyButton.SetAsLastSibling();
        }

        private void OnItemClick(MineSceneUiItemsPanelItem selectedItem)
        {
            if (_isOpened)
                SelectItem(selectedItem);
            else
                UseItem(selectedItem);
        }

        private void SelectItem(MineSceneUiItemsPanelItem selectedItem)
        {
            if (_selectedItem == selectedItem)
            {
                Toggle();
                return;
            }

            foreach (var item in _items)
                item.UpdateState(_isOpened);

            _selectedItem = selectedItem;
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.LastUsedItem, _selectedItem.ItemStaticId);
            _selectedItem.UpdateState(_isOpened, true);
            _title.text = LocalizationHelper.GetLocale(_selectedItem.ItemStaticId);
        }

        private void UseItem(MineSceneUiItemsPanelItem selectedItem)
        {
            if (_ui.IsUiClickable)
                _hero.UseItem(selectedItem.ItemStaticId);
        }
    }
}