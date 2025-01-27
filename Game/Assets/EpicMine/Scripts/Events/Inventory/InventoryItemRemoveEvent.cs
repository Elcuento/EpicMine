using CommonDLL.Dto;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct InventoryItemRemoveEvent
    {
        public Item Item;
        public SpendType SpendType;

        public InventoryItemRemoveEvent(Item item, SpendType spendType)
        {
            Item = item;
            SpendType = spendType;
        }
    }
}