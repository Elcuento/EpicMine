namespace AMTServerDLL.Dto
{
    public class RequestDataBuyPickaxe : SendData
    {
        public string Id;

        public RequestDataBuyPickaxe(string id)
        {
            Id = id;
        }
    }
}