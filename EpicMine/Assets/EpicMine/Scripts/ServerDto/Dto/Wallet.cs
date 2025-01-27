using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class Wallet
    {
        public List<Currency> Currencies;

        public Wallet()
        {
            Currencies = new List<Currency>();
        }

        public Wallet(BlackTemple.EpicMine.Core.Wallet data)
        {
            Currencies = new List<Currency>();

            foreach (var dataCurrency in data.Currencies)
            {
                Currencies.Add(new Currency(dataCurrency.Type, dataCurrency.Amount));
            }
        }
    }
}