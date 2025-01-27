namespace AMTServerDLL.Dto
{
    public class RequestDataPvpFindUser : SendData
    {
        public string UserName;

        public RequestDataPvpFindUser(string userName)
        {
            UserName = userName;
        }
    }
}