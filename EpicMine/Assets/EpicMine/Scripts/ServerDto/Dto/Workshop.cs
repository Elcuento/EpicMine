using System.Collections.Generic;
using CommonDLL.Static;

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

        public Workshop(BlackTemple.EpicMine.Core.Workshop data)
        {
            Recipes = new List<Recipe>();
            SlotsShard = new List<WorkshopSlot>();
            Slots = new List<WorkshopSlot>();

            foreach (var rec in data.Recipes)      
            {
                Recipes.Add(new Recipe(rec.StaticRecipe.Id,new List<string>(rec.FoundResources)));
            }

            foreach (var workshopSlot in data.Slots)
            {
                Slots.Add(new WorkshopSlot()
                {
                   ItemId = workshopSlot.StaticRecipe?.Id,
                   NecessaryAmount = workshopSlot.NecessaryAmount,
                   IsUnlocked = workshopSlot.IsUnlocked,
                   Type = workshopSlot.StaticRecipe?.Type ?? RecipeType.Smelt,
                   MeltingStartTime = workshopSlot.MeltingStartTime
                });
            }
            foreach (var workshopSlot in data.SlotsShard)
            {
                SlotsShard.Add(new WorkshopSlot()
                {
                    ItemId = workshopSlot.StaticRecipe?.Id,
                    NecessaryAmount = workshopSlot.NecessaryAmount,
                    IsUnlocked = workshopSlot.IsUnlocked,
                    Type = workshopSlot.StaticRecipe?.Type ?? RecipeType.Smelt,
                    MeltingStartTime = workshopSlot.MeltingStartTime
                });
            }
        }

       
    }
}