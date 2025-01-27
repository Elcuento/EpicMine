namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateSelectedTorch : SendData
    {
        public string Id;

        public RequestDataUpdateSelectedTorch(string id)
        {
            Id = id;
        }
    }
}