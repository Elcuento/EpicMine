using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataWorkshopSetSlot : SendData
    {
        public int Number;

        public RecipeType Type;

        public WorkShopSlotType SlotType;

        public string Item;

        public int NecessaryAmount;

        public RequestDataWorkshopSetSlot(int number, WorkShopSlotType slotType, RecipeType type, string item, int necessaryAmount)
        {
            Number = number;
            Type = type;
            Item = item;
            SlotType = slotType;
            NecessaryAmount = necessaryAmount;
        }

        
    }
 
}