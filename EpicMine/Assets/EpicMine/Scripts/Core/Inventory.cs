using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;


namespace BlackTemple.EpicMine.Core
{
    public class Inventory
    {
        public List<Item> Items { get; }


        public Inventory()
        {
            Items = new List<Item>();
        }

        public Inventory(CommonDLL.Dto.Inventory inventoryGameDataResponse)
        {
            Items = new List<Item>();

            if (inventoryGameDataResponse.Items != null)
            {
                foreach (var item in inventoryGameDataResponse.Items)
                    Items.Add(new Item(item.Id, item.Amount));
            }
        }


        public int GetExistAmount(string itemId)
        {
            var existItem = Items.FirstOrDefault(i => i.Id == itemId);
            return existItem?.Amount ?? 0;
        }

        public bool Has(Item item)
        {
            var existAmount = GetExistAmount(item.Id);
            return existAmount >= item.Amount;
        }

        public bool Has(List<Item> items)
        {
            return items.All(Has);
        }

        public bool Has(Pack pack)
        {
            return pack.Items.All(Has);
        }


        public void Add(Item item, IncomeSourceType incomeSourceType)
        {
            if (item.Amount <= 0)
                return;

            var existItemIndex = Items.FindIndex(i => i.Id == item.Id);
            if (existItemIndex >= 0)
            {
                var newAmount = Items[existItemIndex].Amount + item.Amount;
                Items[existItemIndex] = new Item(item.Id, newAmount);

                var itemChangeEvent = new InventoryItemChangeEvent(Items[existItemIndex], differenceAmount: item.Amount, isAdded: true, incomeSourceType: incomeSourceType);
                EventManager.Instance.Publish(itemChangeEvent);
            }
            else
            {
                Items.Add(item);

                var newItemAddEvent = new InventoryNewItemAddEvent(item);
                EventManager.Instance.Publish(newItemAddEvent);
            }

            var itemAddEvent = new InventoryItemAddEvent(item, incomeSourceType);
            EventManager.Instance.Publish(itemAddEvent);
        }

        public void Add(List<Item> items, IncomeSourceType incomeSourceType)
        {
            foreach (var dtoItem in items)
                Add(dtoItem, incomeSourceType);
        }

        public void Add(Dictionary<string, int> items, IncomeSourceType incomeSourceType)
        {
            foreach (var dtoItem in items)
                Add(new Item(dtoItem.Key,dtoItem.Value), incomeSourceType);
        }

        public void Add(Pack pack, IncomeSourceType incomeSourceType)
        {
            foreach (var dtoItem in pack.Items)
                Add(dtoItem, incomeSourceType);
        }


        public bool Remove(Item item, SpendType spendType)
        {
            var existItemIndex = Items.FindIndex(i => i.Id == item.Id);
            if (existItemIndex < 0)
                return false;

            if (Items[existItemIndex].Amount < item.Amount)
                return false;

            var newAmount = Items[existItemIndex].Amount - item.Amount;
            if (newAmount <= 0)
            {
                Items.RemoveAt(existItemIndex);
                var itemRemoveExistEvent = new InventoryItemRemoveExistEvent(item.Id, item.Amount, spendType);
                EventManager.Instance.Publish(itemRemoveExistEvent);
            }
            else
            {
                Items[existItemIndex] = new Item(item.Id, newAmount);
                var itemChangeEvent = new InventoryItemChangeEvent(Items[existItemIndex], differenceAmount: item.Amount, spendType: spendType);
                EventManager.Instance.Publish(itemChangeEvent);
            }

            var itemRemoveEvent = new InventoryItemRemoveEvent(item, spendType);
            EventManager.Instance.Publish(itemRemoveEvent);

            return true;
        }

        public bool Remove(List<Item> items, SpendType spendType)
        {
            foreach (var item in items)
            {
                var existItemIndex = Items.FindIndex(i => i.Id == item.Id);
                if (existItemIndex < 0)
                    return false;

                if (Items[existItemIndex].Amount < item.Amount)
                    return false;
            }

            foreach (var dtoItem in items)
            {
                Remove(dtoItem, spendType);
            }

            return true;
        }

        public bool Remove(Pack pack, SpendType spendType)
        {
            return Remove(pack.Items, spendType);
        
        }
    }
}