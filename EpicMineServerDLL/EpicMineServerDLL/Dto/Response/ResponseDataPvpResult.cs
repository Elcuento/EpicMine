using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpResult : SendData
    {
        public PvpArenaMatchInfo Info;

        public ResponseDataPvpResult(PvpArenaMatchInfo info)
        {
            Info = info;
        }
    }
}