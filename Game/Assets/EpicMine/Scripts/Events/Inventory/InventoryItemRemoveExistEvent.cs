

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct InventoryItemRemoveExistEvent
    {
        public string ItemId;
        public int Amount;
        public SpendType SpendType;

        public InventoryItemRemoveExistEvent(string itemId, int amount, SpendType spendType)
        {
            ItemId = itemId;
            Amount = amount;
            SpendType = spendType;
        }
    }
}