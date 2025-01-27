
using CommonDLL.Dto;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct InventoryItemChangeEvent
    {
        public Item Item;
        public int DifferenceAmount;
        public bool IsAdded;
        public IncomeSourceType? IncomeSourceType;
        public SpendType? SpendType;

        public InventoryItemChangeEvent(Item item, int differenceAmount, bool isAdded = false, IncomeSourceType? incomeSourceType = null, SpendType? spendType = null)
        {
            Item = item;
            DifferenceAmount = differenceAmount;
            IsAdded = isAdded;
            IncomeSourceType = incomeSourceType;
            SpendType = spendType;
        }
    }
}