using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct RecipeChangeEvent
    {
        public Recipe Recipe;

        public RecipeChangeEvent(Recipe recipe)
        {
            Recipe = recipe;
        }
    }
}