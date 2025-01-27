using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaOpponentSectionPassedEvent
    {
        public int Number;
        public PvpArenaOpponentSectionPassedEvent(int number)
        {
            Number = number;
        }
    }
}