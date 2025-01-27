using System.Collections.Generic;
using CommonDLL.Dto;

namespace BlackTemple.EpicMine
{
    public struct PvpUpdateChangeEvent
    {
        public Pvp PvpData;

        public PvpUpdateChangeEvent(Pvp pvpData)
        {
            PvpData = pvpData;
        }
    }
}