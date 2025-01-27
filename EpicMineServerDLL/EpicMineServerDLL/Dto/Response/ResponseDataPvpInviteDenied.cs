namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpInviteDenied : SendData
    {
        public string UserId;

        public ResponseDataPvpInviteDenied( string userId)
        {
            UserId = userId;
        }
    }
}