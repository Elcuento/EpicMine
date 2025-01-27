using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpCreate : SendData
    {
        public PvpArenaMatchInfo Data;

        public ResponseDataPvpCreate(PvpArenaMatchInfo data)
        {
            Data = data;
        }
    }
}