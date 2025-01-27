namespace AMTServerDLL.Dto
{
    public class RequestDataChestOpenTutorial : SendData
    {
        public string Id;

        public RequestDataChestOpenTutorial(string id)
        {
            Id = id;
        }
    }
}