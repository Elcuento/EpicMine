namespace AMTServerDLL.Dto
{
    public class RequestDataWorkShopSlotCollectCrystalPay : SendData
    {
        public int Number;

        public RequestDataWorkShopSlotCollectCrystalPay(int number)
        {
            Number = number;
        }
    }
}