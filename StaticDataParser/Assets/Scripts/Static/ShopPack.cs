using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class ShopPack
    {
        public string Id { get; }

        public ShopPackType Type { get; }

        public Dictionary<string, int> Items { get; }

        public List<string> Buffs { get; }

        public Dictionary<CurrencyType, int> Currency { get; }

        public List<int> Amounts { get; }

        public int Cost { get; }

        public int CrystalCost { get; }

        public int SalePercent { get; }

        public int? Charge { get; }

        public int? Time { get; }

        public string ExtraAttribute { get;  }

        public ShopPack(string id, ShopPackType type, string items, string buffs, string currency, string amounts,
            int cost, float crystalCost, int salePercent, int? charge, int? time, string extraAttribute)
        {

            Id = string.IsNullOrEmpty(id) ? "" : id.ToLower();
            Type = type;

            Buffs = !string.IsNullOrEmpty(buffs) ? buffs.Split('#').ToList() : new List<string>();
            Items = Extensions.GetDictionaryBySplitKeyValuePair<string, int>(items);
            Currency = Extensions.GetDictionaryBySplitKeyValuePair<CurrencyType, int>(currency);
            Amounts = !string.IsNullOrEmpty(amounts)
                ? new List<int>(Array.ConvertAll(amounts.Split(','), int.Parse))
                : new List<int>();

            CrystalCost = (int)crystalCost;
            Cost = cost;
            SalePercent = salePercent;
            Charge = charge ?? 0;
            Time = time ?? 0;
            ExtraAttribute = extraAttribute;
        }
    }


}