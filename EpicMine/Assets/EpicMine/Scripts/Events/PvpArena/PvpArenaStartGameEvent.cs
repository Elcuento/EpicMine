using CommonDLL.Dto;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaStartGameEvent
    {
        public PvpArenaMatchInfo Info;

        public PvpArenaStartGameEvent(PvpArenaMatchInfo info)
        {
            Info = info;
        }
    }
}