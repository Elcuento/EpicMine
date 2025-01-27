using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataChestOpenForce : SendData
    {
        public ChestType Type;

        public string Id;

        public RequestDataChestOpenForce(string id, ChestType level)
        {
            Id = id;
            Type = level;
        }
    }
 
}