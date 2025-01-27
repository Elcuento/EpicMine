using System;
using System.Collections.Generic;
using System.Linq;
using CommonDLL.Dto;
using CommonDLL.Static;

namespace EpicMineServerDLL.Static.Helpers
{
    public static class PvpHelper
    {
        public static int GetLeagueNumberByRating(StaticData data, int rating)
        {
            var count = data.Leagues.Count;
            var league = 0;
            for (var i = 0; i < count; i++)
            {
                if (data.Leagues[i].Rating <= rating)
                    league = i;
            }

            return league;
        }

        public static League GetLeagueByRating(StaticData data, int rating)
        {
            var count = data.Leagues.Count;
            var league = data.Leagues[0];
            for (var i = 0; i < count; i++)
            {
                if (data.Leagues[i].Rating <= rating)
                    league = data.Leagues[i];
            }

            return league;
        }

        public static List<int> GetWallsHealths(StaticData data, int lastOpenTier, int leagueNumber, int count)
        {
            var tier = lastOpenTier + 1;

            if (tier <= 1)
                tier = 2;

            var wallHealths = new int[count];

            var leagueSettings =
                data.LeaguesSettings.FirstOrDefault(x => tier >= x.TierMin && tier <= x.TierMax) ??
                data.LeaguesSettings.Last();

            var league = data.Leagues[leagueNumber];

            var averageHealth = leagueSettings.AverageWallHealth[leagueNumber];

            for (var i = 0; i < wallHealths.Length; ++i)
            {
                var wallSettings = data.MineWalls[i];
                wallHealths[i] = (int)(averageHealth * wallSettings.HealthCoefficient * league.Coefficient);
            }

            return wallHealths.ToList();
        }

        public static LeaguesSettings GetSuiteSettings(StaticData data, int lastOpenTier)
        {
            var tier = lastOpenTier + 1;

            if (tier <= 1)
                tier = 2;

            var leagueSettings =
                data.LeaguesSettings.FirstOrDefault(x => tier >= x.TierMin && tier <= x.TierMax) ??
                data.LeaguesSettings.Last();

            return leagueSettings;
        }

        public static int GetSuiteSettingsNumber(StaticData data, int lastOpenTier)
        {
            var tier = lastOpenTier + 1;

            if (tier <= 1)
                tier = 2;

            var leagueSettings =
                data.LeaguesSettings.FirstOrDefault(x => tier >= x.TierMin && tier <= x.TierMax) ??
                data.LeaguesSettings.Last();

            return data.LeaguesSettings.IndexOf(leagueSettings);
        }

        public static bool SuiteRating(PvpArenaMatchInfo pvpArenaMatch, Player playerData)
        {
            if (playerData.Pvp.Rating >= pvpArenaMatch.Filter.MinRating &&
                playerData.Pvp.Rating <= pvpArenaMatch.Filter.MaxRating && 
                playerData.Prestige == pvpArenaMatch.Filter.Prestige)
                return true;

            return false;
        }

        public static bool SuiteFilter(PvpArenaMatchFilter filter, PvpArenaMatchFilter arenaDataFilter)
        {
            
            return Math.Abs(Math.Abs(filter.MinRating) - Math.Abs(arenaDataFilter.MinRating)) < 200 &&
                   Math.Abs(Math.Abs(filter.MaxRating) - Math.Abs(arenaDataFilter.MaxRating)) < 200 &&
                   filter.LeagueSettingsNumber == arenaDataFilter.LeagueSettingsNumber &&
                   filter.Prestige == arenaDataFilter.Prestige;
        }


     
    }
}
