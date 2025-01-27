using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;

using CommonDLL.Static;

namespace BlackTemple.EpicMine.Core
{
    public class Wallet
    {
        public List<Currency> Currencies { get; }

        public Wallet(CommonDLL.Dto.Wallet data)
        {
            Currencies = new List<Currency>();

            if (data.Currencies != null)
            {
                foreach (var currency in data.Currencies)
                {
                    CurrencyType type;

                    switch (currency.Type)
                    {
                        case CommonDLL.Static.CurrencyType.Gold:
                            type = CurrencyType.Gold;
                            break;
                        case CommonDLL.Static.CurrencyType.Crystals:
                            type = CurrencyType.Crystals;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    Currencies.Add(new Currency(type, currency.Amount));
                }
            }
            else
            {
                Currencies.Add(new Currency(CurrencyType.Gold, App.Instance.StaticData.Configs.StartPack.Currencies.Gold));
                Currencies.Add(new Currency(CurrencyType.Crystals, App.Instance.StaticData.Configs.StartPack.Currencies.Crystals));
            }
        }

        public long GetExistAmount(CurrencyType currencyType)
        {
            var existItem = Currencies.FirstOrDefault(i => i.Type == currencyType);
            return existItem.Amount;
        }

        public bool Has(Currency currency)
        {
            var existAmount = GetExistAmount(currency.Type);
            return existAmount >= currency.Amount;
        }
        public bool Has(CurrencyType type, long val)
        {
            var c = new Currency(type, val);

            return Has(c);
        }

        public bool Has(List<Currency> currencies)
        {
            return currencies.All(Has);
        }

        public bool Has(Pack pack)
        {
            return pack.Currencies.All(Has);
        }
        public bool SubsTractCurrency(CurrencyType type, long amount)
        {
            var c = new Currency(type, amount);
            if (Has(c))
            {
                Remove(c);
                return true;
            }

            return false;
        }
        public bool SubsTractCurrency(Currency currency)
        {
            if (Has(currency))
            {
                Remove(currency);
                return true;
            }

            return false;
        }

        public void Add(CurrencyType type, long amount, IncomeSourceType source)
        {
            Add(new Currency(type,amount),source);
        }
        public void Add(Currency currency, IncomeSourceType incomeSourceType, string annotations = "")
        {
            if (currency.Amount <= 0)
                return;

            var currencyCount = Currencies.Find(x => x.Type == currency.Type).Amount;

            var existCurrencyIndex = Currencies.FindIndex(i => i.Type == currency.Type);
            if (existCurrencyIndex >= 0)
            {
                var newAmount = Currencies[existCurrencyIndex].Amount + currency.Amount;
                Currencies[existCurrencyIndex] = new Currency(currency.Type, newAmount);

                var changeEvent = new CurrencyChangeEvent(Currencies[existCurrencyIndex]);
                EventManager.Instance.Publish(changeEvent);
            }
            else
            {
                Currencies.Add(currency);

                var changeEvent = new CurrencyChangeEvent(currency);
                EventManager.Instance.Publish(changeEvent);
            }

            var addEvent = new CurrencyAddEvent(currency, incomeSourceType);
            EventManager.Instance.Publish(addEvent);

            if (currency.Type == CurrencyType.Crystals)
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.CrystalsIncome);

            var accrualType = CurrencyAccrualType.Earned;
            if (incomeSourceType == IncomeSourceType.FromBuy)
                accrualType = CurrencyAccrualType.Purchased;

            if (currency.Type == CurrencyType.Crystals)
            {
                App.Instance.Services.AnalyticsService.CurrencyChange((int) currencyCount,
                    (int) Currencies.Find(x => x.Type == currency.Type).Amount, incomeSourceType, annotations);
            }

            App.Instance.Services.AnalyticsService.CurrencyAccrual((int)currency.Amount, currency.Type.ToString(), accrualType);
        }

        public void Add(List<Currency> currencies, IncomeSourceType incomeSourceType)
        {
            foreach (var currency in currencies)
            {
                Add(currency, incomeSourceType);
            }
        }

        public void Add(Pack pack, IncomeSourceType incomeSourceType)
        {
            foreach (var currency in pack.Currencies)
                Add(currency, incomeSourceType);
        }


        public bool Remove(Currency currency)
        {
            var currencyCount = Currencies.Find(x => x.Type == currency.Type).Amount;

            var existItemIndex = Currencies.FindIndex(i => i.Type == currency.Type);
            if (existItemIndex < 0)
                return false;

            if (Currencies[existItemIndex].Amount < currency.Amount)
                return false;

            var newAmount = Currencies[existItemIndex].Amount - currency.Amount;
            var newCurrency = new Currency(currency.Type, newAmount);
            if (newAmount <= 0)
                Currencies.RemoveAt(existItemIndex);
            else
                Currencies[existItemIndex] = newCurrency;

            var changeEvent = new CurrencyChangeEvent(newCurrency);
            EventManager.Instance.Publish(changeEvent);

            if (currency.Type == CurrencyType.Crystals)
            {
                App.Instance.Services.AnalyticsService.CurrencyChange((int)currencyCount,
                    (int)Currencies.Find(x => x.Type == currency.Type).Amount, IncomeSourceType.None);
            }

            return true;
        }

        public bool Remove(List<Currency> currencies)
        {
            foreach (var currency in currencies)
            {
                var existCurrencyIndex = Currencies.FindIndex(i => i.Type == currency.Type);
                if (existCurrencyIndex < 0)
                    return false;

                if (Currencies[existCurrencyIndex].Amount < currency.Amount)
                    return false;
            }

            foreach (var currency in currencies)
                Remove(currency);

            return true;
        }

        public bool Remove(Pack pack)
        {
            return Remove(pack.Currencies);
        }

        public void SetCurrency(CurrencyType type, long count)
        {
            var index = Currencies.FindIndex(x => x.Type == type);
            if (index != -1)
            {
                Currencies[index] = new Currency(type, count);
            }
            else
            {
                Currencies.Add(new Currency(type, count));
                index = Currencies.FindIndex(x => x.Type == type);
            }
        
            var changeEvent = new CurrencyChangeEvent(Currencies[index]);
            EventManager.Instance.Publish(changeEvent);
        }
    }
}