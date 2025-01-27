using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class RequestDataPvpUpdatePlayerInfo : SendData
    {
        public PvpArenaUserInfo Info;

        public RequestDataPvpUpdatePlayerInfo(PvpArenaUserInfo info)
        {
            Info = info;
        }
    }
}