namespace BlackTemple.EpicMine.Static
{
    public class ShopChest
    {
        public string Id { get; }

        public ShopChestType Type { get; }

        public Rarity? Hilt1Rarity { get; }

        public float Hilt1Chance { get; }

        public Rarity? Hilt2Rarity { get; }

        public float Hilt2Chance { get; }

        public Rarity? Hilt3Rarity { get; }

        public float Hilt3Chance { get; }

        public ShopChest(string id, ShopChestType type, Rarity? hilt1Rarity, float hilt1Chance, Rarity? hilt2Rarity,
            float hilt2Chance, Rarity? hilt3Rarity, float hilt3Chance)
        {
            Id = id;
            Type = type;
            Hilt1Rarity = hilt1Rarity;
            Hilt1Chance = hilt1Chance;
            Hilt2Rarity = hilt2Rarity;
            Hilt2Chance = hilt2Chance;
            Hilt3Rarity = hilt3Rarity;
            Hilt3Chance = hilt3Chance;
        }
    }
}