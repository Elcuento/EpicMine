namespace AMTServerDLL.Dto
{
    public class ResponseDataAddShopSaleCharge : SendData
    {
        public long Date;
        public int Charge;
        public int BuyCharge;

        public ResponseDataAddShopSaleCharge(long date, int charge, int buyCharge)
        {
            Date = date;
            Charge = charge;
            BuyCharge = buyCharge;
        }
    }
}