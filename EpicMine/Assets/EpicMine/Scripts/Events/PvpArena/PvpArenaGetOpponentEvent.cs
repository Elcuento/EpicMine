using CommonDLL.Dto;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaGetOpponentEvent
    {
        public PvpArenaUserInfo Data;

        public PvpArenaGetOpponentEvent(PvpArenaUserInfo data)
        {
            Data = data;
        }
    }
}