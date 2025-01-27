namespace AMTServerDLL.Dto
{
    public class RequestDataPvpInviteAccepted : SendData
    {
        public string UserId;
        public string MatchId;

        public RequestDataPvpInviteAccepted(string userId, string matchId)
        {
            UserId = userId;
            MatchId = matchId;
        }
    }
}