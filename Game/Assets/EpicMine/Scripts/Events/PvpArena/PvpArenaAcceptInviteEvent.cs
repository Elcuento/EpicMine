using CommonDLL.Dto;


namespace BlackTemple.EpicMine
{
    public struct PvpArenaAcceptInviteEvent
    {
        public PvpArenaUserInfo Info;

        public PvpArenaAcceptInviteEvent(PvpArenaUserInfo info)
        {
            Info = info;
        }
    }
}