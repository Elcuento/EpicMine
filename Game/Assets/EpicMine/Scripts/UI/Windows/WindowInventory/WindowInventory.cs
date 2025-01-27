using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.UI;


namespace BlackTemple.EpicMine
{
    public class WindowInventory : WindowBase
    {
        [SerializeField] private WindowInventoryItem _itemPrefab;
        [SerializeField] private WindowInventoryActiveItem _activeItemPrefab;

        [Space]
        [SerializeField] private RectTransform _itemsContainer;
        [SerializeField] private WindowInventoryItemInfoPanel _itemInfoPanel;
        [SerializeField] private Toggle _showAll;
        [SerializeField] private Toggle _showOres;
        [SerializeField] private Toggle _showIngots;
        [SerializeField] private Toggle _showHilts;
        [SerializeField] private Toggle _showItems;

        [Space]
        [SerializeField] private RedDotBaseView _oresRedDot;
        [SerializeField] private RedDotBaseView _ingotsRedDot;
        [SerializeField] private RedDotBaseView _hiltsRedDot;
        [SerializeField] private RedDotBaseView _itemsRedDot;

        private readonly List<WindowInventoryItem> _items = new List<WindowInventoryItem>();
        private WindowInventoryActiveItem _activeItem;


        public void FilterItems()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            foreach (var item in _items)
                CheckItem(item);
        }

 
        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            if (_activeItem == null)
                _activeItem = Instantiate(_activeItemPrefab, _itemsContainer, false);

            App.Instance.Player.Inventory.Items.Sort(SortByPrice);

            foreach (var i in App.Instance.Player.Inventory.Items)
            {
                var item = Instantiate(_itemPrefab, _itemsContainer, false);
                item.Initialize(i.Id, i.Amount, OnItemClick);
                _items.Add(item);
            }

            if (_items.Count <= 0)
                return;

            FilterItems();

            var firstVisibleItem = _items.FirstOrDefault(i => i.gameObject.activeSelf);
            if (firstVisibleItem != null)
                OnItemClick(firstVisibleItem);

            EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
            EventManager.Instance.Subscribe<InventoryItemChangeEvent>(OnItemChange);
            EventManager.Instance.Subscribe<InventoryItemRemoveExistEvent>(OnItemRemove);

            if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier))
            {
                OnRedDotInventoryChange(App.Instance.Controllers.RedDotsController.ViewedItems);
                App.Instance.Controllers.RedDotsController.OnInventoryChange += OnRedDotInventoryChange;
            }
        }

        public override void OnClose()
        {
            base.OnClose();

            foreach (var item in _items)
                Destroy(item.gameObject);

            _items.Clear();

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
                EventManager.Instance.Unsubscribe<InventoryItemChangeEvent>(OnItemChange);
                EventManager.Instance.Unsubscribe<InventoryItemRemoveExistEvent>(OnItemRemove);
            }

            if (App.Instance != null)
            {
                App.Instance.Controllers.RedDotsController.OnInventoryChange -= OnRedDotInventoryChange;
            }
        }


        private void OnItemAdd(InventoryItemAddEvent eventData)
        {
            ChangeItem(eventData.Item);
        }

        private void OnItemChange(InventoryItemChangeEvent eventData)
        {
            ChangeItem(eventData.Item);
        }

        private void OnItemRemove(InventoryItemRemoveExistEvent existEventData)
        {
            ChangeItem(new Item(existEventData.ItemId, 0));
        }


        private void ChangeItem(Item changedItem)
        {
            var item = _items.FirstOrDefault(i => i.StaticItemId == changedItem.Id);

            if (item == null)
            {
                var newItem = Instantiate(_itemPrefab, _itemsContainer, false);
                newItem.Initialize(changedItem.Id, changedItem.Amount, OnItemClick);
                CheckItem(newItem);
                _items.Add(newItem);
                return;
            }

            if (changedItem.Amount <= 0)
            {
                _items.Remove(item);
                Destroy(item.gameObject);

                if (_activeItem.Target.StaticItemId == changedItem.Id && _items.Count > 0)
                    OnItemClick(_items.FirstOrDefault(i => i.Amount > 0));
            }
            else
            {
                var amount = App.Instance.Player.Inventory.GetExistAmount(changedItem.Id);
                item.UpdateAmount(amount);
                
            }
        }

        private void OnItemClick(WindowInventoryItem item)
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _itemInfoPanel.Initialize(item.StaticItemId, item.Amount);
            _activeItem.Initialize(item);
            _activeItem.transform.SetAsLastSibling();
        }

        private void CheckItem(WindowInventoryItem item)
        {
            if (_showAll.isOn)
            {
                item.gameObject.SetActive(true);
                return;
            }

            var resource = App.Instance.StaticData.Resources.FirstOrDefault(i => i.Id == item.StaticItemId);
            if (resource != null)
            {
                switch (resource.Type)
                {
                    case ResourceType.Ore:
                        item.gameObject.SetActive(_showOres.isOn);
                        break;
                    case ResourceType.Ingot:
                        item.gameObject.SetActive(_showIngots.isOn);
                        break;
                    case ResourceType.Shard:
                        item.gameObject.SetActive(_showIngots.isOn);
                        break;
                    case ResourceType.Item:
                        item.gameObject.SetActive(_showOres.isOn);
                        break;
                    case ResourceType.QuestItem:
                        item.gameObject.SetActive(_showOres.isOn);
                        break;
                }

                return;
            }

            var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(i => i.Id == item.StaticItemId);
            if (hilt != null)
            {
                item.gameObject.SetActive(_showHilts.isOn);
                return;
            }
            
            item.gameObject.SetActive(_showItems.isOn);
        }

        private int SortByPrice(Item x, Item y)
        {
            var xPrice = StaticHelper.GetItemPrice(x.Id);
            var yPrice = StaticHelper.GetItemPrice(y.Id);

            if (xPrice < yPrice)
                return 1;

            if (xPrice == yPrice)
                return 0;

            return -1;
        }

        private void OnRedDotInventoryChange(List<string> viewedItems)
        {
            StartCoroutine(UpdateRedDots(viewedItems));
        }

        private IEnumerator UpdateRedDots(List<string> viewedItems, float delay = 0.05f)
        {
            yield return new WaitForSecondsRealtime(delay);

            var oresCount = 0;
            var ingotsCount = 0;
            var hiltsCount = 0;
            var itemsCount = 0;

            foreach (var inventoryItem in _items)
            {
                inventoryItem.HideRedDot();
                if (!viewedItems.Contains(inventoryItem.StaticItemId))
                {
                    inventoryItem.ShowRedDot();

                    var resource = App.Instance.StaticData.Resources.FirstOrDefault(i => i.Id == inventoryItem.StaticItemId);
                    if (resource != null)
                    {
                        switch (resource.Type)
                        {
                            case ResourceType.Ore:
                                oresCount++;
                                break;
                            case ResourceType.Ingot:
                                ingotsCount++;
                                break;
                        }

                        continue;
                    }

                    var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(i => i.Id == inventoryItem.StaticItemId);
                    if (hilt != null)
                    {
                        hiltsCount++;
                        continue;
                    }

                    itemsCount++;
                }
            }

            _oresRedDot.Show(oresCount);
            _ingotsRedDot.Show(ingotsCount);
            _hiltsRedDot.Show(hiltsCount);
            _itemsRedDot.Show(itemsCount);
        }
    }
}