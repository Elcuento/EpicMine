using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataWorkShopSlotComplete : SendData
    {
        public int Number;

        public WorkShopSlotType SlotType;

        public RequestDataWorkShopSlotComplete(int number, WorkShopSlotType type)
        {
            Number = number;
            SlotType = type;
        }
    }
 
}