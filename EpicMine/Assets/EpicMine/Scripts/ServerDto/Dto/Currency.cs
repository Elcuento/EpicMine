using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class Currency
    {
        public CurrencyType Type;

        public long Amount;

        public Currency(CurrencyType type, long amount)
        {
            Type = type;
            Amount = amount;
        }
    }

}