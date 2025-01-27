using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpInviteAccepted : SendData
    {
        public PvpArenaUserInfo UserInfo;
        public PvpArenaMatchInfo MatchInfo;

        public ResponseDataPvpInviteAccepted(PvpArenaUserInfo userInfo, PvpArenaMatchInfo matchInfo)
        {
            UserInfo = userInfo;
            MatchInfo = matchInfo;
        }
    }
}