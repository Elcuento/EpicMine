namespace AMTServerDLL.Dto
{
    public class ResponseDataSpendCrystals : SendData
    {
        public long Quantity;

        public ResponseDataSpendCrystals(long quantity)
        {
            Quantity = quantity;
        }

    }
}