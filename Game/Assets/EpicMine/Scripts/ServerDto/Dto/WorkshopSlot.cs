using System;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class WorkshopSlot
    {
        public string ItemId;

        public DateTime? MeltingStartTime;

        public RecipeType Type;

        public int NecessaryAmount;

        public bool IsUnlocked;
    }
}