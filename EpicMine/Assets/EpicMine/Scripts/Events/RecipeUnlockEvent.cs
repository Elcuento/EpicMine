using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct RecipeUnlockEvent
    {
        public Recipe Recipe;

        public RecipeUnlockEvent(Recipe recipe)
        {
            Recipe = recipe;
        }
    }
}