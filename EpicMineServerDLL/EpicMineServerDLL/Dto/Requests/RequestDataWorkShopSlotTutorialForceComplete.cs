namespace AMTServerDLL.Dto
{
    public class RequestDataWorkShopSlotTutorialForceComplete : SendData
    {
        public int Number;

        public RequestDataWorkShopSlotTutorialForceComplete(int number)
        {
            Number = number;
        }
    }
}