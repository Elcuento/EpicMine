namespace AMTServerDLL.Dto
{
    public class RequestDataSetNickName : SendData
    {
        public string NickName;

        public RequestDataSetNickName(string nickName)
        {
            NickName = nickName;
        }
    }
}