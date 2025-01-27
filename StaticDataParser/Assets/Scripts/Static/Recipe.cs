namespace BlackTemple.EpicMine.Static
{
    public class Recipe
    {
        public string Id { get; }

        public RecipeType Type { get; }

        public int Amount { get; }

        public float CraftTime { get; }

        public string Ingredient1Id { get; }

        public int Ingredient1Amount { get; }

        public string Ingredient2Id { get; }

        public int Ingredient2Amount { get; }

        public string Ingredient3Id { get; }

        public int Ingredient3Amount { get; }

        public int FilterCategory { get; }

        public Recipe(string id, RecipeType type, int amount, float craftTime, string ingredient1Id, int ingredient1Amount, string ingredient2Id,
            int ingredient2Amount, string ingredient3Id, int ingredient3Amount, int filterCategory)
        {
            Id = id.ToLower();
            Type = type;
            Amount = amount;
            CraftTime = craftTime;
            Ingredient1Id = ingredient1Id.ToLower();
            Ingredient1Amount = ingredient1Amount;
            Ingredient2Id = ingredient2Id.ToLower();
            Ingredient2Amount = ingredient2Amount;
            Ingredient3Id = ingredient3Id.ToLower();
            Ingredient3Amount = ingredient3Amount;
            FilterCategory = filterCategory;
        }
    }
}