namespace AMTServerDLL.Dto
{
    public class RequestDataShopSubscriptionRestore : SendData
    {
        public string Id;

        public long ExpireDate;

        public RequestDataShopSubscriptionRestore(string id, long expireDate)
        {
            Id = id;
            ExpireDate = expireDate;
        }
    }
}