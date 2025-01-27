using System.Collections.Generic;

namespace CommonDLL.Static
{
    public class ShopPack
    {
        public string Id ;

        public ShopPackType Type ;

        public Dictionary<string, int> Items ;

        public List<string> Buffs ;

        public Dictionary<CurrencyType, int> Currency ;

        public List<int> Amounts ;

        public int Cost ;

        public int CrystalCost ;

        public int SalePercent ;

        public int? Charge ;

        public int? Time ;

        public string ExtraAttribute;


    }


}