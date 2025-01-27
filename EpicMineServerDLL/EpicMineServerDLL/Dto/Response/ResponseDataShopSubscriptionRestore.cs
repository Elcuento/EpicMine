namespace AMTServerDLL.Dto
{
    public class ResponseDataShopSubscriptionRestore : SendData
    {
        public long ExpireDate;

        public ResponseDataShopSubscriptionRestore(long expireDate)
        {
            ExpireDate = expireDate;
        }
    }
}