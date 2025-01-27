namespace AMTServerDLL.Dto
{
    public class RequestDataAddShopOffer : SendData
    {
        public string Id;

        public RequestDataAddShopOffer(string id)
        {
            Id = id;
        }
    }
}