namespace BlackTemple.EpicMine.Static
{
    public class ChestCommonSettings
    {
        public ChestType Type { get; }

        public int TierNumber { get; }

        public int AMineNumber { get; }

        public int BMineNumber { get; }

        public int CMineNumber { get; }

        public int TierOffset { get; }

        public float DPercent { get; }

        public float E1Percent { get; }

        public ChestCommonSettings(ChestType type, int tierNumber, int aMineNumber, int bMineNumber, int cMineNumber,
            int tierOffset, float dPercent, float e1Percent)
        {
            Type = type;
            TierNumber = tierNumber;
            AMineNumber = aMineNumber;
            BMineNumber = bMineNumber;
            CMineNumber = cMineNumber;
            TierOffset = tierOffset;
            DPercent = dPercent;
            E1Percent = e1Percent;
        }
    }
}