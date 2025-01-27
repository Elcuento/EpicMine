using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataChestOpen : SendData
    {
        public string Id;

        public RequestDataChestOpen(string id)
        {
            Id = id;
        }
    }
 
}