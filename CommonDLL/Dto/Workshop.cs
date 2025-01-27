using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class Workshop
    {
        public List<Recipe> Recipes;

        public List<WorkshopSlot> Slots;

        public List<WorkshopSlot> SlotsShard;

        public Workshop()
        {
            Recipes = new List<Recipe>();
            SlotsShard= new List<WorkshopSlot>();
            Slots = new List<WorkshopSlot>();
        }

       
    }
}