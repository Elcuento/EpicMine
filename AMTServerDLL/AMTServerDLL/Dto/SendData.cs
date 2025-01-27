namespace AMTServerDLL.Dto
{
    public class SendData
    {
        public SendDataError Error;

        public SendData(SendDataError error)
        {
            Error = error;
        }

        public SendData()
        {

        }
    }
}