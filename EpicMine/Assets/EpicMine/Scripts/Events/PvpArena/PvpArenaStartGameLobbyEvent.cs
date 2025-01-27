using CommonDLL.Dto;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaStartGameLobbyEvent
    {
        public PvpArenaMatchInfo Info;

        public PvpArenaStartGameLobbyEvent(PvpArenaMatchInfo info)
        {
            Info = info;
        }
    }
}