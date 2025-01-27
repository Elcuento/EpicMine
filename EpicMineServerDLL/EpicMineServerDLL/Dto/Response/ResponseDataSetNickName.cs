namespace AMTServerDLL.Dto
{
    public class ResponseDataSetNickName : SendData
    {
        public string Nick;

        public ResponseDataSetNickName(string nick)
        {
            Nick = nick;
        }
    }
}