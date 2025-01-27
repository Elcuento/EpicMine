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
    }
}