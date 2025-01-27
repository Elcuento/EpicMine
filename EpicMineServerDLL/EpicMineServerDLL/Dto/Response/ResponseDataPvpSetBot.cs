using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpSetBot : SendData
    {
        public PvpArenaUserInfo UserInfo;

        public ResponseDataPvpSetBot(PvpArenaUserInfo userInfo)
        {
            UserInfo = userInfo;
        }
    }
}