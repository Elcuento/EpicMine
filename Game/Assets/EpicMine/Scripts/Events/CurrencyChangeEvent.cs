using BlackTemple.EpicMine.Dto;

namespace BlackTemple.EpicMine
{
    public struct CurrencyChangeEvent
    {
        public Currency Currency;

        public CurrencyChangeEvent(Currency currency)
        {
            Currency = currency;
        }
    }
}