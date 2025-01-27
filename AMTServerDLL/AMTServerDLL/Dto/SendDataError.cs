namespace AMTServerDLL.Dto
{
    public class SendDataError
    {
        public int ErrorCode;

        public SendDataError(int errorCode = 0)
        {
            ErrorCode = errorCode;
        }
    }
}