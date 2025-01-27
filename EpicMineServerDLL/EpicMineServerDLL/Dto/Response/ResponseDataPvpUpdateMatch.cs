using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpUpdateMatchInfo : SendData
    {
        public PvpArenaMatchInfo Data;

        public ResponseDataPvpUpdateMatchInfo(PvpArenaMatchInfo data)
        {
            Data = data;
        }
    }
}