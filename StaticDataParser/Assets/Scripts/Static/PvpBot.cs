using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class PvpBot
    {
        public string Id { get; }

        public string[] Names { get; }

        public int League { get; }

        public int BreakWallTimeMin { get; }

        public int BreakWallTimeMax { get; }

        public int BreakWallTimeMinLimit { get; }

        public int MinRatingOffset { get; }

        public int MaxRatingOffset { get; }

        public int EmoStartChance { get; }

        public int EmoPassWallChance { get; }

        public int EmoEndChance { get; }

        public PvpBot(string id, string names, int league, int breakWallTimeMin, int breakWallTimeMax, int breakWallTimeMinLimit, int minRatingOffset, int maxRatingOffset, string emoChances)
        {
            Id = id.ToLower();
            Names = names.Split(',');
            League = league;
            BreakWallTimeMax = breakWallTimeMax;
            BreakWallTimeMin = breakWallTimeMin;
            BreakWallTimeMinLimit = breakWallTimeMinLimit;
            MinRatingOffset = minRatingOffset;
            MaxRatingOffset = maxRatingOffset;
            var chances = emoChances.Split(',');
            EmoStartChance = int.Parse(chances[0]);
            EmoPassWallChance = int.Parse(chances[1]);
            EmoEndChance = int.Parse(chances[2]);
        }
    }
}