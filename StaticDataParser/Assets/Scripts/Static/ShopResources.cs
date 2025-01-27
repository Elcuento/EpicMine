namespace BlackTemple.EpicMine.Static
{
    public class ShopResources
    {
        public string Id { get; }

        public int CrystalCost { get; }

        public int MinCount { get; }


        public ShopResources(string id, int crystalCost, int minCount)
        {
            Id = id.ToLower();
            CrystalCost = crystalCost;
            MinCount = minCount;
        }

    }
}