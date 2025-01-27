namespace AMTServerDLL.Dto
{
    public class RequestDataPvpInviteCancel : SendData
    {
        public string Id;

        public RequestDataPvpInviteCancel(string id)
        {
            Id = id;
        }
    }
}