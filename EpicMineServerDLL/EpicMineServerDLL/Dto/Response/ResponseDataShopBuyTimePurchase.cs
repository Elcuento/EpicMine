namespace AMTServerDLL.Dto
{
    public class ResponseDataShopBuyTimePurchase : SendData
    {
        public int Charge;

        public long Date;

        public ResponseDataShopBuyTimePurchase(long date, int charge)
        {
            Charge = charge;
            Date = date;
        }
    }
}