namespace BlackTemple.EpicMine.Static
{
    public class WorkshopSlot
    {
        public CurrencyType PriceCurrencyType { get; }

        public int PriceAmount { get; }

        public WorkshopSlot(CurrencyType priceCurrencyType, int priceAmount)
        {
            PriceCurrencyType = priceCurrencyType;
            PriceAmount = priceAmount;
        }
    }
}