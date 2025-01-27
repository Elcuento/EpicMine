using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpJoinCreateArena : SendData
    {
        public PvpArenaMatchInfo Data;

        public ResponseDataPvpJoinCreateArena(PvpArenaMatchInfo data)
        {
            Data = data;
        }
    }
}