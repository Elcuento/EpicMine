using System;

namespace BlackTemple.EpicMine.Static
{
    public class LeaguesSettings
    {
        public int[] AverageWallHealth { get; }

        public int TierMax { get; }

        public int TierMin { get; }

        public LeaguesSettings(string averageWallHealth, int tierMax, int tierMin)
        {
            AverageWallHealth = !string.IsNullOrEmpty(averageWallHealth)
                ? (Array.ConvertAll(averageWallHealth.Split(','), int.Parse)) : new int[0];

            TierMin = tierMin;
            TierMax = tierMax;
        }
    }
}