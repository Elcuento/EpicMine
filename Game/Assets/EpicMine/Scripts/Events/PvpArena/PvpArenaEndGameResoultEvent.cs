using System.Collections.Generic;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public struct PvpArenaEndGameResoultEvent
    {
        public PvpArenaGameResoultType Resoult;

        public int PlayerRating;
        public int OpponentRating;

        public int PlayerChangeRating;
        public int OpponentChangeRating;

        public PvpArenaEndGameResoultEvent(PvpArenaGameResoultType resoult,int playerRating, int opponentRating, int playerChangeRating, int opponentChangeRating)
        {
            Resoult = resoult;
            PlayerRating = playerRating;
            OpponentRating = opponentRating;
            PlayerChangeRating = playerChangeRating;
            OpponentChangeRating = opponentChangeRating;
        }
    }
}