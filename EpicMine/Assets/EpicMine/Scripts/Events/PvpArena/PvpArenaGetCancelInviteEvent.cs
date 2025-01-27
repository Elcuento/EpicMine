using CommonDLL.Dto;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaGetCancelInviteEvent
    {
        public bool TimeOut;

        public PvpArenaUserInfo Info;

        public PvpArenaGetCancelInviteEvent(PvpArenaUserInfo info, bool isTimeOut)
        {
            Info = info;
            TimeOut = isTimeOut;
        }
    }
}