using System;
using System.Collections.Generic;
using System.Linq;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine.Purchasing;
using Currency = BlackTemple.EpicMine.Dto.Currency;
using Random = UnityEngine.Random;


namespace BlackTemple.EpicMine
{
    public static class ShopHelper
    {
        public static ShopPack GetPackByProductId(Product product)
        {
            var config = App.Instance.StaticData.Configs.Shop.Iaps.Find(x => x.Id == product.definition.id);
            return App.Instance.StaticData.ShopPacks.Find(x => x.Id == config.Pack);
        }

        public static Product GetProductByPackId(string packId)
        {
#if UNITY_EDITOR
            var packConfig = App.Instance.StaticData.Configs.Shop.Iaps.Find(x => x.Pack == packId);
#else
   var packConfig = App.Instance.StaticData.Configs.Shop.Iaps.Find(x => x.Pack == packId && (x.Platform == App.Instance.CurrentPlatform || x.Platform == PlatformType.All));
#endif

            return InAppPurchaseManager.Instance.StoreController.products.all.FirstOrDefault(x =>
                x.definition.id == packConfig.Id);
        }

        public static int GetPackCrystalsCost(ShopPack pack)
        {
            var cost = 0;

            foreach (var packItem in pack.Items)
            {
                var res = App.Instance.StaticData.ShopResources.Find(x => x.Id == packItem.Key);
                if (res != null)
                {
                    var count = packItem.Value / res.MinCount;
                    cost += (res.CrystalCost * count);
                }
            }

            cost += pack.Currency.ContainsKey(CurrencyType.Crystals) ? pack.Currency[CurrencyType.Crystals] : 0;

            return cost;
        }

        public static int GetCrystalsItemsCost(Dictionary<string, int> items)
        {
            var cost = 0;

            foreach (var packItem in items)
            {
                var res = App.Instance.StaticData.ShopResources.Find(x =>
                    x.Id == packItem.Key || x.Id == packItem.Key.Replace("ore", "ingot"));
                if (res != null)
                {

                    var count = packItem.Value / res.MinCount;
                    cost += (res.CrystalCost * count);
                }
            }

            return cost;
        }

        public static float GetResourceSalePercent(int amount)
        {
            if (amount < 10)
                return 0;

            var maxSale = App.Instance.StaticData.Configs.Shop.ResourceMaxSale;

            var sale = (amount / 300f) * maxSale;
            return sale <= 0.3f ? sale : 0.3f;
        }

        public static long GetCurrentGoldShopPackAmount(int gold)
        {
            int coefficient;
            var currentTier = App.Instance.Player.Dungeon.Tiers.LastOrDefault(t => t.IsOpen);

            if (currentTier == null || currentTier.Number < 5)
                coefficient = App.Instance.StaticData.Configs.Shop.GoldTiersDozenMultipliers[0];
            else if (currentTier.Number < 10)
                coefficient = App.Instance.StaticData.Configs.Shop.GoldTiersDozenMultipliers[1];
            else if (currentTier.Number < 15)
                coefficient = App.Instance.StaticData.Configs.Shop.GoldTiersDozenMultipliers[2];
            else if (currentTier.Number < 20)
                coefficient = App.Instance.StaticData.Configs.Shop.GoldTiersDozenMultipliers[3];
            else if (currentTier.Number < 25)
                coefficient = App.Instance.StaticData.Configs.Shop.GoldTiersDozenMultipliers[4];
            else if (currentTier.Number < 30)
                coefficient = App.Instance.StaticData.Configs.Shop.GoldTiersDozenMultipliers[5];
            else if (currentTier.Number < 35)
                coefficient = App.Instance.StaticData.Configs.Shop.GoldTiersDozenMultipliers[6];
            else if (currentTier.Number < 40)
                coefficient = App.Instance.StaticData.Configs.Shop.GoldTiersDozenMultipliers[7];
            else
                coefficient = App.Instance.StaticData.Configs.Shop.GoldTiersDozenMultipliers[8];

            return gold * coefficient;
        }

        public static ShopPack GetSalePackByLevel(ShopPack pack, int charge)
        {
            if (pack.Type == ShopPackType.Miner)
                return App.Instance.StaticData.ShopPacks.Find(x => x.Id == $"shop_pack_miner_{(charge + 1)}");

            if (pack.Type == ShopPackType.Alchemy)
                return App.Instance.StaticData.ShopPacks.Find(x => x.Id == $"shop_pack_alchemy_{(charge + 1)}");

            return App.Instance.StaticData.ShopPacks.Find(x => x.Id == $"shop_pack_dragon_{(charge + 1)}");
        }

        public static Dictionary<string, int> GetShopPackItems(ShopPack pack)
        {
            var items = new Dictionary<string, int>();

            var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number;
            var simpleChest = App.Instance.StaticData.SimpleChests[tier];

            var keys = pack.Items.Keys.ToList();

            var dropList = new List<string>
            {
                simpleChest.A1ItemId,
                simpleChest.A2ItemId,
                simpleChest.B1ItemId,
                simpleChest.B2ItemId
            };

            foreach (var t in keys)
            {
                if (t.Contains("random"))
                {
                    if (dropList.Count > 0)
                    {
                        items.Add(dropList[0], pack.Items[t]);
                        dropList.Remove(dropList[0]);
                    }
                }
                else
                {
                    items.Add(t, pack.Items[t]);
                }
            }

            return items;
        }

        public static Item GetRandomHilt(Rarity rarity)
        {
            var availableHilts = StaticHelper.GetHiltsByPickaxeRare(rarity).Where(h => h.DropCategory > 0).ToList();

            var randomIndex = Random.Range(0, availableHilts.Count);
            var randomHilt = availableHilts[randomIndex];
            return new Item(randomHilt.Id, 1);

        }

        public static void ExtractBuffs(ShopPack pack, long timeEnd, bool withCheck = false)
        {
            var buffsAdd = new List<Core.Buff>();

            var player = App.Instance.Player;
            var buffs = pack.Buffs;

            foreach (var buffId in buffs)
            {
                var buff = player.Effect.AddExchangeBuff(buffId, timeEnd);
                buffsAdd.Add(buff);

                if (withCheck)
                    player.Effect.CheckBuff(buffId);
            }

            var buffAdded = buffsAdd.Find(x => x.Type == BuffType.Currency);

            if (buffAdded != null)
            {
                if (buffAdded.Value.ContainsKey(BuffValueType.CrystalsByDay))
                {
                    WindowManager.Instance.Show<WindowCrystalSubscription>()
                        .Initialize(buffAdded);
                }
            }
        }


        public static void ExtractShopPack(ShopPack pack, bool noBuff = false)
        {
            var itemsAdd = new List<Item>();
            var buffsAdd = new List<Core.Buff>();
            var currencyAdd = new List<Currency>();

            var player = App.Instance.Player;

            var items = GetShopPackItems(pack);

            foreach (var packItem in items)
            {
                var item = new Item(packItem.Key, packItem.Value);
                player.Inventory.Add(item, IncomeSourceType.FromShopBuy);
                itemsAdd.Add(item);
            }

            foreach (var packItem in pack.Currency)
            {
                if (packItem.Key == CurrencyType.Gold)
                {
                    long amount;

                    if (pack.Type == ShopPackType.Gold && pack.CrystalCost == 0)
                    {
                        var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number;
                        amount = (int) (StaticHelper.GetChestGoldAmount(tier, false) * 0.5f);
                    }
                    else
                    {
                        amount = GetCurrentGoldShopPackAmount(packItem.Value);
                    }

                    var cur = new Currency(packItem.Key, amount);

                    player.Wallet.Add(cur, IncomeSourceType.FromShopBuy);
                    currencyAdd.Add(cur);
                }
                else
                {
                    var extraCrystals = 0;

                    if (App.Instance.GameEvents.IsActive(GameEventType.BlackFriday))
                    {
                        if (pack.Type == ShopPackType.Crystals)
                            extraCrystals += packItem.Value / 2;
                    }

                    var cur = new Currency(packItem.Key, (long) (packItem.Value + extraCrystals));
                    player.Wallet.Add(cur, IncomeSourceType.FromShopBuy, pack.Id);
                    currencyAdd.Add(cur);
                }
            }

            if (!noBuff)
            {
                foreach (var buffId in pack.Buffs)
                {
                    var buff = player.Effect.AddExchangeBuff(buffId);
                    buffsAdd.Add(buff);
                }
            }

            if (pack.Type == ShopPackType.Hilt)
            {
                var item = pack.Id == "shop_pack_hilt_1" ? GetRandomHilt(Rarity.Simple) :
                    pack.Id == "shop_pack_hilt_2" ? GetRandomHilt(Rarity.Rare) :
                    GetRandomHilt(Rarity.Legendary);

                player.Inventory.Add(item, IncomeSourceType.FromBuy);
                itemsAdd.Add(item);
            }


            if (itemsAdd.Count > 0 || currencyAdd.Count > 0)
                WindowManager.Instance.Show<WindowShopPackReward>()
                    .Initialize(itemsAdd, currencyAdd);

            if (buffsAdd.Count > 0)
            {
                var buff = buffsAdd.Find(x => x.Type == BuffType.Boost);
                var maxBuffTime = App.Instance.Player.Effect.GetMaxTimeBuff(buff);
                if (buff != null)
                {
                    if (buff.Value.ContainsKey(BuffValueType.Melting))
                    {
                        WindowManager.Instance.Show<WindowInformation>()
                            .Initialize(
                                buff.Id,
                                $"{LocalizationHelper.GetLocale("left")} \n {TimeHelper.SecondsToDate(maxBuffTime.TimeLeft, true, true)}",
                                "OK",
                                isNeedLocalizeDescription: false,
                                isNeedLocalizeButton: false);

                    }
                    else if (buff.Value.ContainsKey(BuffValueType.Resource))
                    {
                        WindowManager.Instance.Show<WindowInformation>()
                            .Initialize(
                                buff.Id,
                                $"{LocalizationHelper.GetLocale("left")} \n {TimeHelper.SecondsToDate(maxBuffTime.TimeLeft, true, true)}",
                                "OK",
                                isNeedLocalizeDescription: false,
                                isNeedLocalizeButton: false);

                    }
                }

                buff = buffsAdd.Find(x => x.Type == BuffType.Currency);

                if (buff != null)
                {
                    if (buff.Value.ContainsKey(BuffValueType.CrystalsByDay))
                    {
                        WindowManager.Instance.Show<WindowCrystalSubscription>()
                            .Initialize(buff);
                    }
                }
            }
        }

        public static void ShowTutorialCrystalsOffer(Action onEnd = null)
        {
            if (App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.ShopStartGameOffer) ||
                !InAppPurchaseManager.Instance.IsInitialized)
            {
                onEnd?.Invoke();
                return;
            }

            if (App.Instance.Player.Shop.HasActiveSubscription())
            {
                WindowManager.Instance.Show<WindowShop>()
                    .Intialize(onEnd);

                App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.ShopStartGameOffer, true);
                return;
            }

            if (App.Instance.CurrentPlatform == PlatformType.IOS)
            {
                WindowManager.Instance.Show<WindowCrystalSubscriptionStartGameOffer>()
                    .Initialize(onEnd);
            }
            else
            {
                WindowManager.Instance.Show<WindowCrystalSubscriptionStartGameOfferAndroid>()
                    .Initialize(onEnd);
            }

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.ShopStartGameOffer, true);
        }



        public static void ShowStartGameShop(Action onEnd = null)
        {
            if (!App.Instance.Controllers.TutorialController.IsComplete ||
                App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.ShopStartGameOffer) ||
                !InAppPurchaseManager.Instance.IsInitialized)
            {
                onEnd?.Invoke();
                return;
            }

            if (App.Instance.Player.Shop.HasActiveSubscription())
            {
                WindowManager.Instance.Show<WindowShop>(withCurrencies:true)
                    .Intialize(onEnd);
            }
            else
            {
                if (App.Instance.CurrentPlatform == PlatformType.IOS)
                {
                    WindowManager.Instance.Show<WindowCrystalSubscriptionStartGameOffer>()
                        .Initialize(onEnd);
                }
                else
                {
                    WindowManager.Instance.Show<WindowCrystalSubscriptionStartGameOfferAndroid>()
                        .Initialize(onEnd);
                }
            }

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.ShopStartGameOffer, true);
        }
    }

}