namespace AMTServerDLL.Dto
{
    public class RequestDataPvpInvite : SendData
    {
        public string UserId;

        public RequestDataPvpInvite(string userId)
        {
            UserId = userId;
        }
    }
}