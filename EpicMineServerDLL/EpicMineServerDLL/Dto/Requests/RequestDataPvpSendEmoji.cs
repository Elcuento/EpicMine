namespace AMTServerDLL.Dto
{
    public class RequestDataPvpSendEmoji : SendData
    {
        public string MatchId;
        public int Id;

        public RequestDataPvpSendEmoji(string matchId, int id)
        {
            MatchId = matchId;
            Id = id;
        }
    }
}