using System.Collections.Generic;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class AutoMinerSpeedLevels
    {
        public Dictionary<string, long> ItemsCost { get; }

        public Dictionary<CurrencyType, long> CurrencyCost { get; }

        public int Amount { get; }

        public int Stage { get; }

        public AutoMinerSpeedLevels(string itemsCost, string currencyCost, int amount, int stage)
        {
            ItemsCost = Extensions.GetDictionaryBySplitKeyValuePair<string, long>(itemsCost, ';');
            CurrencyCost = Extensions.GetDictionaryBySplitKeyValuePair<CurrencyType, long>(currencyCost,';');
            Amount = amount;
            Stage = stage;
        }
    }
}