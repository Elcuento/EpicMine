using System.Collections.Generic;
using System.Linq;
using BlackTemple.EpicMine.Core;
using CommonDLL.Dto;

namespace BlackTemple.EpicMine.Dto
{
    public class Pack
    {
        public List<Item> Items = new List<Item>();

        public List<Currency> Currencies = new List<Currency>();

        public long Artefacts;

        public int TotalItemsCount => Items.Sum(x => x.Amount);


        public Pack() { }

        public void Multiplay(int x)
        {
            var items = new List<Item>();
            var currencies = new List<Currency>();

            foreach (var item in Items)
            {
                var itemPack = item;
                itemPack.Amount = item.Amount * x;
                items.Add(itemPack);
            }
            foreach (var currency in Currencies)
            {
                var itemPack = currency;
                itemPack.Amount = currency.Amount * x;
                currencies.Add(itemPack);
            }

            Items = items;
            Currencies = currencies;
        }

        public Pack(Dictionary<string, int> items)
        {
            Items = new List<Item>();

            foreach (var item in items)
            {
                Items.Add( new Item(item.Key, item.Value));
            }
        }

        public Pack(List<Item> items)
        {
            Items = items;
        }

        public Pack(List<Currency> currencies)
        {
            Currencies = currencies;
        }

        public Pack(Artifacts artefacts)
        {
            Artefacts = artefacts.Amount;
        }

        public Pack(List<Item> items, List<Currency> currencies, Artifacts artefacts)
        {
            Items = items;
            Currencies = currencies;
            Artefacts = artefacts.Amount;
        }


        public void Add(Item item)
        {
            var existItemIndex = Items.FindIndex(i => i.Id == item.Id);
            if (existItemIndex >= 0)
            {
                var existAmount = Items[existItemIndex].Amount;
                Items[existItemIndex] = new Item(item.Id, existAmount + item.Amount);
                return;
            }

            Items.Add(item);
        }

        public void Add(Currency currency)
        {
            var existCurrencyIndex = Currencies.FindIndex(i => i.Type == currency.Type);
            if (existCurrencyIndex >= 0)
            {
                var existAmount = Currencies[existCurrencyIndex].Amount;
                Currencies[existCurrencyIndex] = new Currency(currency.Type, existAmount + currency.Amount);
                return;
            }

            Currencies.Add(currency);
        }

        public void Add(long artifacts)
        {
            Artefacts += artifacts;
        }
    }
}