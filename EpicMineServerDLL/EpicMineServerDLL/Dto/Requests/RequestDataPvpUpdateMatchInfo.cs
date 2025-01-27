using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class RequestDataPvpUpdateMatchInfo : SendData
    {
        public PvpArenaMatchInfo Info;

        public RequestDataPvpUpdateMatchInfo(PvpArenaMatchInfo info)
        {
            Info = info;
        }
    }
}