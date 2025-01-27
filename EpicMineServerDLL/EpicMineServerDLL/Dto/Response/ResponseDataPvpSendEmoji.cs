namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpSendEmoji : SendData
    {
        public string MatchId;
        public int Id;

        public ResponseDataPvpSendEmoji(string matchId, int id)
        {
            MatchId = matchId;
            Id = id;
        }
    }
}