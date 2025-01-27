using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataChestAdd : SendData
    {
        public ChestType Type;

        public int Level;

        public RequestDataChestAdd(ChestType type, int level)
        {
            Type = type;
            Level = level;
        }
    }
 
}