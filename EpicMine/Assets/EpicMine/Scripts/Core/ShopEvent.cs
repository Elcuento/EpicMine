using System.Collections.Generic;
using CommonDLL.Static;


namespace BlackTemple.EpicMine.Core
{
    public class ShopEvent
    {
        public string Id;
        public Dictionary<CurrencyType, int> RequireCurrency;

        public ShopEvent(Dto.ShopSale data)
        {
            Id = data.Id;

        }
    }
}