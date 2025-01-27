using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataPvpChestOpen : SendData
    {
        public PvpChestType Type;

        public bool IsInventory;

        public RequestDataPvpChestOpen(PvpChestType type, bool isInventory)
        {
            Type = type;
            IsInventory = isInventory;
        }
    }
}