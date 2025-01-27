namespace BlackTemple.EpicMine.Static
{
    public class MineCommonChance
    {
        public bool IsOddTier { get; }

        public float Item1Percent { get; }

        public float Item2Percent { get; }

        public float Item3Percent { get; }

        public float ChestPercent { get; }

        public MineCommonChance(bool isOddTier, float item1Percent, float item2Percent, float item3Percent, float chestPercent)
        {
            IsOddTier = isOddTier;
            Item1Percent = item1Percent;
            Item2Percent = item2Percent;
            Item3Percent = item3Percent;
            ChestPercent = chestPercent;
        }
    }
}