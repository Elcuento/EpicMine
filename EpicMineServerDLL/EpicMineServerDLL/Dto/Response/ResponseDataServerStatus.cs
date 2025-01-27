namespace AMTServerDLL.Dto
{

    public class ResponseDataServerStatus : SendData
    {
        public bool Status;

        public ResponseDataServerStatus(bool status)
        {
            Status = status;
        }
    }
}