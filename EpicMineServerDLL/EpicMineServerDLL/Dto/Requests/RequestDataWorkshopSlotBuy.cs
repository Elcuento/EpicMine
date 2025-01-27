using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataWorkshopSlotBuy : SendData
    {
        public int Number;

        public WorkShopSlotType SlotType;

        public RequestDataWorkshopSlotBuy(int number, WorkShopSlotType type)
        {
            Number = number;
            SlotType = type;
        }
    }
 
}