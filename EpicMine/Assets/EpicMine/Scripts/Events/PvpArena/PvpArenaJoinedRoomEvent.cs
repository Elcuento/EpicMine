using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaJoinedRoomEvent
    {
        public string GameInfo;
        public PvpArenaJoinedRoomEvent(string info = "")
        {
            GameInfo = info;
        }
    }
}