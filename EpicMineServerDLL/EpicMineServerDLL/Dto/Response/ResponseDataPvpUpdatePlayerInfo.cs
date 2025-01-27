using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpUpdatePlayerInfo : SendData
    {
        public PvpArenaUserInfo Info;

        public ResponseDataPvpUpdatePlayerInfo(PvpArenaUserInfo info)
        {
            Info = info;
        }
    }
}