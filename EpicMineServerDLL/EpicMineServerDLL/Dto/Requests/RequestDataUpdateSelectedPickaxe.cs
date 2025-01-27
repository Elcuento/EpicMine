namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateSelectedPickaxe : SendData
    {
        public string Id;

        public RequestDataUpdateSelectedPickaxe(string id)
        {
            Id = id;
        }
    }
}