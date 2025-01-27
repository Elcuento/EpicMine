namespace AMTServerDLL.Dto
{
    public class ResponseDataWorkShopSlotTutorialForceComplete : SendData
    {
        public int CollectedAmount;

        public int CurrencySpend;

        public ResponseDataWorkShopSlotTutorialForceComplete(int collectedAmount, int currencySpend)
        {
            CollectedAmount = collectedAmount;
            CurrencySpend = currencySpend;
        }
    }
}