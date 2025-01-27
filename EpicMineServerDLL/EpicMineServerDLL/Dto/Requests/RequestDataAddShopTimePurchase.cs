namespace AMTServerDLL.Dto
{
    public class RequestDataAddShopTimePurchase : SendData
    {
        public string Id;

        public RequestDataAddShopTimePurchase(string id)
        {
            Id = id;
        }
    }
}