using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpInvite : SendData
    {
        public PvpArenaUserInfo UserInfo;
        public PvpArenaMatchInfo MatchInfo;

        public ResponseDataPvpInvite(PvpArenaMatchInfo matchInfo, PvpArenaUserInfo userInfo)
        {
            MatchInfo = matchInfo;
            UserInfo = userInfo;
        }
    }
}