using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaPlayerLeaveGameEvent
    {
        public string PlayerName;
        public PvpArenaPlayerLeaveGameEvent(string name = "")
        {
            PlayerName = name;
        }
    }
}