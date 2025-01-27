using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataWorkShopSlotCollect : SendData
    {
        public int Number;

        public int NeedAmount;

        public bool WithStop;

        public WorkShopSlotType SlotType;

        public RequestDataWorkShopSlotCollect(int number, int needAmount, bool withStop, WorkShopSlotType type)
        {
            Number = number;
            NeedAmount = needAmount;
            SlotType = type;
            WithStop = withStop;
        }
    }
 
}