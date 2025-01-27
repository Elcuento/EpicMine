using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaFailedJoinedCustomRoomEvent
    {
        public string GameInfo;
        public PvpArenaFailedJoinedCustomRoomEvent(string info = "")
        {
            GameInfo = info;
        }
    }
}