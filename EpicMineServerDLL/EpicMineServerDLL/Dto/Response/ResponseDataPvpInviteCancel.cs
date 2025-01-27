using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpInviteCancel : SendData
    {
        public PvpArenaUserInfo UserInfo;

        public ResponseDataPvpInviteCancel( PvpArenaUserInfo userInfo)
        {
            UserInfo = userInfo;
        }
    }
}