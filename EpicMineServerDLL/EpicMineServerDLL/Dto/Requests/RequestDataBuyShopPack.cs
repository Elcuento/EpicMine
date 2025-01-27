namespace AMTServerDLL.Dto
{
    public class RequestDataBuyShopPack : SendData
    {
        public string Id;
        public string Receipt;

        public RequestDataBuyShopPack(string id, string receipt)
        {
            Id = id;
            Receipt = receipt;
        }
    }
}