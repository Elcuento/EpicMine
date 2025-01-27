using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaGetEmodjiEvent
    {
        public int EmodjiNumber;

        public PvpArenaGetEmodjiEvent(int eNumber)
        {
            EmodjiNumber = eNumber;
        }
    }
}