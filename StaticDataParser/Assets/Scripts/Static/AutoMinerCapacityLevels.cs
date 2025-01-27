using System.Collections.Generic;
using BlackTemple.Common;

namespace BlackTemple.EpicMine.Static
{
    public class AutoMinerCapacityLevels
    {
        public Dictionary<string, long> ItemsCost { get; }

        public Dictionary<CurrencyType, long> CurrencyCost { get; }

        public int Capacity { get; }

        public AutoMinerCapacityLevels(string itemsCost, string currencyCost, int capacity)
        {
            ItemsCost = Extensions.GetDictionaryBySplitKeyValuePair<string, long>(itemsCost,'#');
            CurrencyCost = Extensions.GetDictionaryBySplitKeyValuePair<CurrencyType, long>(currencyCost, '#');
            Capacity = capacity;
        }
    }
}