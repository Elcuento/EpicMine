using System;
using CommonDLL.Static;


namespace BlackTemple.EpicMine.Dto
{
    public struct WorkshopSlot
    {
        public string ItemId;

        public int NecessaryAmount;

        public DateTime? MeltingStartTime;

        public RecipeType Type;


        public WorkshopSlot(string itemId, RecipeType type, int necessaryAmount, DateTime? meltingStartTime)
        {
            ItemId = itemId.ToLower();
            NecessaryAmount = necessaryAmount;
            MeltingStartTime = meltingStartTime;
            Type = type;
        }
    }
}