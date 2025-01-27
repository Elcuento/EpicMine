using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;

namespace BlackTemple.EpicMine.Core
{
    public class Recipe
    {
        public CommonDLL.Static.Recipe StaticRecipe { get; }

        public readonly List<string> FoundResources;

        public bool IsUnlocked
        {
            get
            {
                if(StaticRecipe.Id.Contains("shard"))
                return true;

                foreach (var ingredient in Ingredients)
                {
                    if (!FoundResources.Contains(ingredient.Id))
                        return false;
                }

                return true;
            }
        }

        public List<Item> Ingredients => StaticHelper.GetIngredients(StaticRecipe);


        public Recipe(CommonDLL.Static.Recipe staticRecipe, CommonDLL.Dto.Recipe dtoRecipe)
        {
            StaticRecipe = staticRecipe;
            FoundResources = dtoRecipe.FoundResources;

            if (!IsUnlocked)
                EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
        }
        public Recipe(CommonDLL.Static.Recipe staticRecipe)
        {
            StaticRecipe = staticRecipe;

            if (!IsUnlocked)
                EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
        }

        public void Unlock()
        {
            if (IsUnlocked)
                return;

            foreach (var ingredient in Ingredients)
            {
                if (!FoundResources.Contains(ingredient.Id))
                {
                    FoundResources.Add(ingredient.Id);
                    EventManager.Instance.Publish(new RecipeChangeEvent(this));
                }
            }

            EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
            EventManager.Instance.Publish(new RecipeUnlockEvent(this));
        }


        private void OnItemAdd(InventoryItemAddEvent eventData)
        {
            if (!Ingredients.Exists(item => item.Id == eventData.Item.Id))
                return;

            if (!FoundResources.Contains(eventData.Item.Id))
            {
                FoundResources.Add(eventData.Item.Id);
                EventManager.Instance.Publish(new RecipeChangeEvent(this));
            }

            if (!IsUnlocked)
                return;

            EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
            EventManager.Instance.Publish(new RecipeUnlockEvent(this));
        }
    }
}