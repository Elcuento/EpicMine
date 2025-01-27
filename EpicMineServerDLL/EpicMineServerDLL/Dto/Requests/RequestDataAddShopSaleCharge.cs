namespace AMTServerDLL.Dto
{
    public class RequestDataAddShopSaleCharge : SendData
    {
        public string Receipt;
        public string Id;

        public RequestDataAddShopSaleCharge(string id, string receipt)
        {
            Receipt = receipt;
            Id = id;
        }
    }
}