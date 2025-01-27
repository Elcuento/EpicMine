namespace AMTServerDLL.Dto
{
    public class RequestDataChestStartBreaking : SendData
    {
        public string Id;

        public RequestDataChestStartBreaking(string id)
        {
            Id = id;
        }
    }
 
}