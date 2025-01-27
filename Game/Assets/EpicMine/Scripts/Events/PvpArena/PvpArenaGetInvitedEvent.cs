using CommonDLL.Dto;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaGetInvitedEvent
    {
        public PvpArenaUserInfo UserInfo;
        public string MatchId;

        public PvpArenaGetInvitedEvent(PvpArenaUserInfo userInfo, string matchId)
        {
            UserInfo = userInfo;
            MatchId = matchId;
        }
    }
}