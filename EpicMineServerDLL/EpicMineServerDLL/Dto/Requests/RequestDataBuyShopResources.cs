namespace AMTServerDLL.Dto
{
    public class RequestDataBuyShopResources : SendData
    {
        public string Id;

        public long Amount;

        public float Sale;

        public RequestDataBuyShopResources(string id, long amount, float sale)
        {
            Id = id;
            Amount = amount;
            Sale = sale;
        }
    }
 
}