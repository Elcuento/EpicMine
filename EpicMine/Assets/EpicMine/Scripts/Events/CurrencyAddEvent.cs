using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public class CurrencyAddEvent
    {
        public Currency Currency;
        public IncomeSourceType IncomeSourceType;

        public CurrencyAddEvent(Currency currency, IncomeSourceType incomeSourceType)
        {
            Currency = currency;
            IncomeSourceType = incomeSourceType;
        }
    }
}