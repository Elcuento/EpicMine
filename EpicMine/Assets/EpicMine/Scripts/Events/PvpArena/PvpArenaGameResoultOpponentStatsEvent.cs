namespace BlackTemple.EpicMine
{
    public struct PvpArenaGameResoultOpponentStatsEvent
    {
        public int League;
        public int Rating;

        public PvpArenaGameResoultOpponentStatsEvent(int league, int rating)
        {
            Rating = rating;
            League = league;
        }
    }
}