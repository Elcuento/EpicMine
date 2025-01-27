using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaTimeTickEvent
    {
        public long ResoultTime;

        public PvpArenaTimeTickEvent(long resoultTime)
        {
            ResoultTime = resoultTime;
        }
    }
}