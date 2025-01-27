using CommonDLL.Dto;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct InventoryItemAddEvent
    {
        public Item Item;
        public IncomeSourceType IncomeSourceType;

        public InventoryItemAddEvent(Item item, IncomeSourceType incomeSourceType)
        {
            Item = item;
            IncomeSourceType = incomeSourceType;
        }
    }
}