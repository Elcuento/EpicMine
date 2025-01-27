namespace BlackTemple.EpicMine.Static
{
    public class Mine
    {
        public int TierNumber { get; }

        public float AverageWallHealth { get; }

        public float? Item1Percent { get; }

        public float? Item2Percent { get; }

        public float? Item3Percent { get; }

        public float? ChestPercent { get; }

        public Mine(int tierNumber, float averageWallHealth, float? item1Percent, float? item2Percent, float? item3Percent, float? chestPercent)
        {
            TierNumber = tierNumber;
            AverageWallHealth = averageWallHealth;
            Item1Percent = item1Percent;
            Item2Percent = item2Percent;
            Item3Percent = item3Percent;
            ChestPercent = chestPercent;
        }
    }
}