using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;

namespace BlackTemple.EpicMine.Core
{
    public class AutoMinerSpeedLevel
    {
        public int Number { get; private set; }

        public AutoMinerSpeedLevels StaticLevel { get; private set; }

        public AutoMinerSpeedLevels NextStaticLevel { get; private set; }


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


        public AutoMinerSpeedLevel(int lvl)
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
                EventManager.Instance.Publish(new AutoMinerChangeSpeedLevelEvent(this));
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


            Number++;
            UpdateStaticLevels();

            if (costPack.Currencies.Count > 0)
            {
                var crystals = costPack.Currencies.Find(x => x.Type == CurrencyType.Crystals);

                if (crystals.Amount > 0)
                    App.Instance.Services.AnalyticsService.CustomEvent("autoMiner_speed_up",
                        new CustomEventParameters
                        {
                            Int = new Dictionary<string, int>
                            {
                                { "crystal_cost", (int)crystals.Amount }
                            }
                        });
            }

            App.Instance.Player.Inventory.Remove(costPack, SpendType.Using);
            App.Instance.Player.Wallet.Remove(costPack);

            EventManager.Instance.Publish(new AutoMinerChangeSpeedLevelEvent(this));
        }


        private void UpdateStaticLevels()
        {
            StaticLevel = StaticHelper.GetAutoMinerSpeedLevel(Number);
            NextStaticLevel = StaticHelper.GetAutoMinerSpeedLevel(Number + 1);
        }
    }
}