using System.Collections.Generic;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class ShopEvent
    {
        public string Id;
        public Dictionary<CurrencyType, int> RequireCurrency;
    }
}