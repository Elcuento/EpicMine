namespace AMTServerDLL.Dto
{

    internal class SendDataPing : SendData
    {
        public string UserId { get; private set; }

        public SendDataPing(string userId)
        {
            UserId = userId;
        }
    }
}