using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;


namespace BlackTemple.EpicMine.Core
{
    public class AutoMinerCapacityLevel
    {
        public int Number { get; private set; }

        public AutoMinerCapacityLevels StaticLevel { get; private set; }

        public AutoMinerCapacityLevels NextStaticLevel { get; private set; }


        public bool CanUpgrade
        {
            get
            {
                if (NextStaticLevel == null)
                    return false;

                var costPack = new Pack();

                if (NextStaticLevel.CurrencyCost != null)
                {
                    foreach (var currency in NextStaticLevel.CurrencyCost)
                    {
                        costPack.Add(new Currency(currency.Key, currency.Value));
                    }
                }
                if (NextStaticLevel.ItemsCost != null)
                {
                    foreach (var item in NextStaticLevel.ItemsCost)
                    {
                        costPack.Add(new Item(item.Key, (int)item.Value));
                    }
                }

                return App.Instance.Player.Inventory.Has(costPack) && App.Instance.Player.Wallet.Has(costPack);
            }
        }


        public AutoMinerCapacityLevel(int lvl)
        {
            Number = lvl;

            UpdateStaticLevels();
        }


        public void Up(bool force = false)
        {
            if (NextStaticLevel == null)
                return;

            if (force)
            {
                Number++;
                UpdateStaticLevels();
                EventManager.Instance.Publish(new AutoMinerChangeCapacityLevelEvent(this));
                return;
            }

            var costPack = new Pack();

            if (NextStaticLevel.CurrencyCost != null)
            {
                foreach (var currency in NextStaticLevel.CurrencyCost)
                {
                    costPack.Add(new Currency(currency.Key, currency.Value));
                }
            }

            if (NextStaticLevel.ItemsCost != null)
            {
                foreach (var item in NextStaticLevel.ItemsCost)
                {
                    costPack.Add(new Item(item.Key, (int)item.Value));
                }
            }

            if (!App.Instance.Player.Inventory.Has(costPack) || !App.Instance.Player.Wallet.Has(costPack))
                return;

            var val = Number + 1;

            var staticData = App.Instance.StaticData;

            long cost = 0;

            foreach (var l in staticData.AutoMinerCapacityLevels[val].CurrencyCost)
            {
                if (l.Key == CurrencyType.Crystals)
                    cost += l.Value;
            }

            Number++;
            UpdateStaticLevels();

            var gold = costPack.Currencies.Find(x => x.Type == CurrencyType.Gold).Amount;

            App.Instance.Player.Inventory.Remove(costPack, SpendType.Using);
            App.Instance.Player.Wallet.Remove(new Currency(CurrencyType.Crystals, cost));
            App.Instance.Player.Wallet.Remove(new Currency(CurrencyType.Gold, gold));

            App.Instance.Services.AnalyticsService.CustomEvent("autoMiner_capacity_up",
                new CustomEventParameters
                {
                    Int = new Dictionary<string, int>
                    {
                        { "crystal_cost", (int)cost }
                    }
                });
            EventManager.Instance.Publish(new AutoMinerChangeCapacityLevelEvent(this));
        }


        private void UpdateStaticLevels()
        {
            StaticLevel = StaticHelper.GetAutoMinerCapacityLevel(Number);
            NextStaticLevel = StaticHelper.GetAutoMinerCapacityLevel(Number +1);
        }
    }
}