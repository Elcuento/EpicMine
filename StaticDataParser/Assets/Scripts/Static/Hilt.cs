namespace BlackTemple.EpicMine.Static
{
    public class Hilt
    {
        public string Id { get; }

        public int Price { get; }

        public float DropChance { get; }

        public int DropCategory { get; }

        public Hilt(string id, int price, float dropChance, int dropCategory)
        {
            Id = id;
            Price = price;
            DropChance = dropChance;
            DropCategory = dropCategory;
        }
    }
}