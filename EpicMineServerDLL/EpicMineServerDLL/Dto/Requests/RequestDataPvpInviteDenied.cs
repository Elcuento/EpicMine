namespace AMTServerDLL.Dto
{
    public class RequestDataPvpInviteDenied : SendData
    {
        public string Id;

        public RequestDataPvpInviteDenied(string id)
        {
            Id = id;
        }
    }
}