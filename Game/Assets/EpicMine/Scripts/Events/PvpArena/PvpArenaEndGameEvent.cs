using System.Collections.Generic;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public struct PvpArenaEndGameEvent
    {
        public string Winner;
        public PvpArenaGameResoultType Resoult;

        public PvpArenaEndGameEvent(string winner, PvpArenaGameResoultType resoult)
        {
            Winner = winner;
            Resoult = resoult;
        }
    }
}