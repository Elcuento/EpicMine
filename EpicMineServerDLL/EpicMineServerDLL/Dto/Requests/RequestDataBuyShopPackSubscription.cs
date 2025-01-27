namespace AMTServerDLL.Dto
{
    public class RequestDataBuyShopPackSubscription : SendData
    {
        public string Receipt;

        public string Id;

        public long Time;

        public RequestDataBuyShopPackSubscription(string receipt, string id, long time )
        {
            Receipt = receipt;
            Id = id;
            Time = time;
        }
    }
 
}