using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using AbilityLevel = CommonDLL.Static.AbilityLevel;
using Currency = BlackTemple.EpicMine.Dto.Currency;
using Random = UnityEngine.Random;
using SkillLevel = CommonDLL.Static.SkillLevel;

namespace BlackTemple.EpicMine
{
    public static class StaticHelper
    {
        public static int GetProgressLevel(int tierNumber, int mineNumber, int prestige = 0)
        {
            var level = tierNumber * LocalConfigs.TierMinesCount + mineNumber + 1;
            if (prestige > 0)
            {
                var levelsCount = App.Instance.StaticData.Tiers.Count * LocalConfigs.TierMinesCount;
                level += prestige * levelsCount;
            }

            return level;
        }

        public static bool IsTypeOf(string id, Type type)
        {
            id = id.ToLower();
            var resource = App.Instance.StaticData.Resources.FirstOrDefault(i => i.Id == id);
            if (resource != null && resource.GetType() == type)
                return true;
            
            var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(i => i.Id == id);
            if (hilt != null && hilt.GetType() == type)
                return true;
            
            var potion = App.Instance.StaticData.Potions.FirstOrDefault(i => i.Id == id);
            if (potion != null && potion.GetType() == type)
                return true;

            var tnt = App.Instance.StaticData.Tnt.FirstOrDefault(i => i.Id == id);
            if (tnt != null && tnt.GetType() == type)
                return true;

            var shopChest = App.Instance.StaticData.ShopChests.FirstOrDefault(i => i.Id == id);
            if (shopChest != null && shopChest.GetType() == type)
                return true;

            return false;
        }

        public static int GetItemPrice(string id)
        {
            var resource = App.Instance.StaticData.Resources.FirstOrDefault(i => i.Id == id);
            if (resource != null)
                return resource.Price;

            var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(i => i.Id == id);
            if (hilt != null)
                return hilt.Price;

            return 0;
        }


        public static CommonDLL.Static.Tier GetTier(int number)
        {
            return App.Instance.StaticData.Tiers[number];
        }

        public static CommonDLL.Static.Mine GetMine(int tierNumber, int mineNumber)
        {
            var mines = App.Instance.StaticData.Mines.Where(mine => mine.TierNumber == tierNumber + 1).ToList();
            return mines[mineNumber];
        }

        public static MineCommonChance GetMineCommonSettings(int tierNumber, int mineNumber)
        {
            var isOddTier = (tierNumber + 1) % 2 != 0;
            var tierMinesCommonSettings = App.Instance.StaticData.MineCommonChances.Where(s => s.IsOddTier == isOddTier).ToList();
            return tierMinesCommonSettings[mineNumber];
        }


        public static List<Item> GetIngredients(CommonDLL.Static.Recipe recipe, int multiply = 1)
        {
            var ingredients = new List<Item>();

            if (!string.IsNullOrEmpty(recipe.Ingredient1Id) && recipe.Ingredient1Amount > 0)
                ingredients.Add(new Item(recipe.Ingredient1Id, recipe.Ingredient1Amount * multiply));

            if (!string.IsNullOrEmpty(recipe.Ingredient2Id) && recipe.Ingredient2Amount > 0)
                ingredients.Add(new Item(recipe.Ingredient2Id, recipe.Ingredient2Amount * multiply));

            if (!string.IsNullOrEmpty(recipe.Ingredient3Id) && recipe.Ingredient3Amount > 0)
                ingredients.Add(new Item(recipe.Ingredient3Id, recipe.Ingredient3Amount * multiply));

            return ingredients;
        }

        public static List<Item> GetIngredients(CommonDLL.Static.Pickaxe pickaxe)
        {
            var ingredients = new List<Item>();

            if (!string.IsNullOrEmpty(pickaxe.Ingredient1Id) && pickaxe.Ingredient1Amount > 0)
                ingredients.Add(new Item(pickaxe.Ingredient1Id, pickaxe.Ingredient1Amount));

            if (!string.IsNullOrEmpty(pickaxe.Ingredient2Id) && pickaxe.Ingredient2Amount > 0)
                ingredients.Add(new Item(pickaxe.Ingredient2Id, pickaxe.Ingredient2Amount));

            if (!string.IsNullOrEmpty(pickaxe.Ingredient3Id) && pickaxe.Ingredient3Amount > 0)
                ingredients.Add(new Item(pickaxe.Ingredient3Id, pickaxe.Ingredient3Amount));

            if (!string.IsNullOrEmpty(pickaxe.Hilt))
                ingredients.Add(new Item(pickaxe.Hilt, 1));

            return ingredients;
        }

        public static List<Item> GetIngredients(CommonDLL.Static.Torch torch)
        {
            var ingredients = new List<Item>();

            if (!string.IsNullOrEmpty(torch.Ingredient1Id) && torch.Ingredient1Amount > 0)
                ingredients.Add(new Item(torch.Ingredient1Id, torch.Ingredient1Amount));

            if (!string.IsNullOrEmpty(torch.Ingredient2Id) && torch.Ingredient2Amount > 0)
                ingredients.Add(new Item(torch.Ingredient2Id, torch.Ingredient2Amount));

            if (!string.IsNullOrEmpty(torch.Ingredient3Id) && torch.Ingredient3Amount > 0)
                ingredients.Add(new Item(torch.Ingredient3Id, torch.Ingredient3Amount));

            if (!string.IsNullOrEmpty(torch.Shard))
                ingredients.Add(new Item(torch.Shard, 1));

            return ingredients;
        }

        public static CommonDLL.Static.Recipe GetRecipe(string itemStaticId)
        {
            return App.Instance.StaticData.Recipes.FirstOrDefault(r => r.Id == itemStaticId);
        }


        public static List<Hilt> GetHiltsByPickaxeType(PickaxeType pickaxeType)
        {
            var hilts = new List<Hilt>();
            var pickaxes = App.Instance.StaticData.Pickaxes.Where(p => p.Type == pickaxeType).ToList();
            if (pickaxes.Count > 0)
            {
                foreach (var pickaxe in pickaxes)
                {
                    var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(h => h.Id == pickaxe.Hilt);
                    if (hilt == null)
                        continue;

                    if (!hilts.Contains(hilt))
                        hilts.Add(hilt);
                }
            }

            return hilts;
        }
        
        public static List<Hilt> GetHiltsByPickaxeRare(Rarity pickaxeRarity)
        {
            var hilts = new List<Hilt>();
            var pickaxes = App.Instance.StaticData.Pickaxes.Where(p => p.Rarity == pickaxeRarity).ToList();
            if (pickaxes.Count > 0)
            {
                foreach (var pickaxe in pickaxes)
                {
                    var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(h => h.Id == pickaxe.Hilt);
                    if (hilt == null)
                        continue;

                    if (!hilts.Contains(hilt))
                        hilts.Add(hilt);
                }
            }

            return hilts;
        }


        public static CommonDLL.Static.Chest GetChest(ChestType chestType, int level)
        {
            switch (chestType)
            {
                case ChestType.Simple:
                    return App.Instance.StaticData.SimpleChests[level];
                case ChestType.Royal:
                    return App.Instance.StaticData.RoyalChests[level];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static ChestConfigs GetChestConfigs(ChestType chestType)
        {
            switch (chestType)
            {
                case ChestType.Simple:
                    return App.Instance.StaticData.Configs.Burglar.Chests[ChestType.Simple];
                case ChestType.Royal:
                    return App.Instance.StaticData.Configs.Burglar.Chests[ChestType.Royal];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static MineMonsterSpawnChance GetMonsterSpawn(int tier, int mine)
        {
            if (App.Instance.StaticData.MineMonstersSpawn.Count > tier)
            {
                var tierSpawn = App.Instance.StaticData.MineMonstersSpawn[tier];
                if (tierSpawn.SpawnChances.Count > mine)
                    return tierSpawn.SpawnChances[mine];
            }

            return null;
        }
        
        public static ChestCommonSettings GetChestCommonSettings(ChestType chestType)
        {
            return App.Instance.StaticData.ChestCommonSettings.FirstOrDefault(ch => ch.Type == chestType);
        }

        public static DateTime GetChestBreakedTime(ChestType chestType, DateTime startBreakingTime)
        {
            return startBreakingTime.AddMinutes(GetChestConfigs(chestType).BreakingTimeInMinutes);
        }

        public static TimeSpan GetChestBreakingTimeLeft(ChestType chestType, DateTime startBreakingTime)
        {
            var time = GetChestBreakedTime(chestType, startBreakingTime) - TimeManager.Instance.Now;
            if (time.TotalSeconds < 0)
                time = TimeSpan.Zero;

            return time;
        }

        public static int GetChestForceCompleteCost(ChestType chestType, DateTime? startBreakingTime)
        {
            var halfHoursAmount = startBreakingTime.HasValue && startBreakingTime.Value > DateTime.MinValue
                ? Mathf.CeilToInt((float)GetChestBreakingTimeLeft(chestType, startBreakingTime.Value).TotalMinutes / 30)
                : Mathf.CeilToInt((float)GetChestConfigs(chestType).BreakingTimeInMinutes / 30);

            return halfHoursAmount * App.Instance.StaticData.Configs.Burglar.ChestForceCompletePricePer30Minutes;
        }

        public static Pack GetChestRandomDrop(ChestType chestType, int level, long droppedCrystals, long droppedArtefacts)
        {
            var pack = new Pack();
            var chestCommonSettings = GetChestCommonSettings(chestType);
            var staticChest = GetChest(chestType, level);

            var tierNumber = chestCommonSettings.TierNumber - 1 + chestCommonSettings.TierOffset;
            tierNumber = Mathf.Clamp(tierNumber, 0, App.Instance.StaticData.Tiers.Count);

            var aMine = GetMine(tierNumber, chestCommonSettings.AMineNumber - 1);
            var aMineCommonSettings = GetMineCommonSettings(tierNumber, chestCommonSettings.AMineNumber - 1);
            var aItem1Percent = aMine.Item1Percent ?? aMineCommonSettings.Item1Percent;
            var aItem2Percent = aMine.Item2Percent ?? aMineCommonSettings.Item2Percent;
            var aSectionChance = Random.Range(0, aItem1Percent + aItem2Percent);
            var aItemId = aSectionChance <= aItem1Percent ? staticChest.A1ItemId : staticChest.A2ItemId;
            var aRandomAmount = Random.Range(staticChest.AAmountMin, staticChest.AAmountMax + 1);
            pack.Add(new Item(aItemId, aRandomAmount));


            var bMine = GetMine(tierNumber, chestCommonSettings.BMineNumber - 1);
            var bMineCommonSettings = GetMineCommonSettings(tierNumber, chestCommonSettings.BMineNumber - 1);
            var bItem1Percent = bMine.Item1Percent ?? bMineCommonSettings.Item1Percent;
            var bItem2Percent = bMine.Item2Percent ?? bMineCommonSettings.Item2Percent;
            var bSectionChance = Random.Range(0, bItem1Percent + bItem2Percent);
            var bItemId = bSectionChance <= bItem1Percent ? staticChest.B1ItemId : staticChest.B2ItemId;
            var bRandomAmount = Random.Range(staticChest.BAmountMin, staticChest.BAmountMax + 1);
            pack.Add(new Item(bItemId, bRandomAmount));


            var cMine = GetMine(tierNumber, chestCommonSettings.CMineNumber - 1);
            var cMineCommonSettings = GetMineCommonSettings(tierNumber, chestCommonSettings.CMineNumber - 1);
            var cItem1Percent = cMine.Item2Percent ?? cMineCommonSettings.Item2Percent;
            var cItem2Percent = cMine.Item3Percent ?? cMineCommonSettings.Item3Percent;
            var cSectionChance = Random.Range(0, cItem1Percent + cItem2Percent);
            var cItemId = cSectionChance <= cItem1Percent ? staticChest.C1ItemId : staticChest.C2ItemId;
            var cRandomAmount = Random.Range(staticChest.CAmountMin, staticChest.CAmountMax + 1);
            pack.Add(new Item(cItemId, cRandomAmount));


            if (droppedArtefacts > 0)
                pack.Add(droppedArtefacts);

            var eSectionChance = Random.Range(0, 100f);
            if (eSectionChance <= chestCommonSettings.E1Percent)
            {
                var randomGoldAmount = Random.Range(staticChest.EAmountMin, staticChest.EAmountMax + 1);
                pack.Add(new Currency(CurrencyType.Gold, randomGoldAmount));
            }

            if (droppedCrystals > 0)
                pack.Add(new Currency(CurrencyType.Crystals, droppedCrystals));
            else
            {
                var hilts = App.Instance.StaticData.Hilts.Where(p => p.DropCategory == staticChest.HiltDropCategory).ToList();
                if (hilts.Count > 0)
                {
                    var randomValue = Random.Range(0, 100f);
                    Hilt droppedHilt = null;
                    var hiltChance = 0f;
                    foreach (var hilt in hilts)
                    {
                        hiltChance += hilt.DropChance;
                        if (randomValue > hiltChance)
                            continue;

                        droppedHilt = hilt;
                        break;
                    }

                    if (droppedHilt != null)
                        pack.Add(new Item(droppedHilt.Id, 1));
                }
            }

            return pack;
        }

        public static Pack GetPvpChestRandomDrop(PvpChestType chestType, int level, int droppedArtefacts = 3)
        {
            var pack = new Pack();
            var pvpChest = chestType == PvpChestType.Simple ? App.Instance.StaticData.PvpSimpleChests[level] :
                chestType == PvpChestType.Royal ? App.Instance.StaticData.PvpRoyalChests[level] :
                App.Instance.StaticData.PvpWinnerChests[level];

            var randomGoldAmount = Random.Range(pvpChest.GoldAmountMin, pvpChest.GoldAmountMax + 1);
            pack.Add(new Currency(CurrencyType.Gold, randomGoldAmount));


            var chestCommonSettings = GetChestCommonSettings(chestType == PvpChestType.Simple
                ? ChestType.Simple : ChestType.Royal);

            var tierNumber = chestCommonSettings.TierNumber - 1 + chestCommonSettings.TierOffset;
            tierNumber = Mathf.Clamp(tierNumber, 0, App.Instance.StaticData.Tiers.Count);

            var aMine = GetMine(tierNumber, chestCommonSettings.AMineNumber - 1);
            var aMineCommonSettings = GetMineCommonSettings(tierNumber, chestCommonSettings.AMineNumber - 1);
            var aItem1Percent = aMine.Item1Percent ?? aMineCommonSettings.Item1Percent;
            var aItem2Percent = aMine.Item2Percent ?? aMineCommonSettings.Item2Percent;
            var aSectionChance = Random.Range(0, aItem1Percent + aItem2Percent);
            var aItemId = aSectionChance <= aItem1Percent
                ? pvpChest.FirstResourceFirstVariant
                : pvpChest.FirstResourceSecondVariant;
            var aRandomAmount = Random.Range(pvpChest.FirstResourceAmountMin, pvpChest.FirstResourceAmountMax + 1);
            pack.Add(new Item(aItemId, aRandomAmount));

            var bMine = GetMine(tierNumber, chestCommonSettings.BMineNumber - 1);
            var bMineCommonSettings = GetMineCommonSettings(tierNumber, chestCommonSettings.BMineNumber - 1);
            var bItem1Percent = bMine.Item1Percent ?? bMineCommonSettings.Item1Percent;
            var bItem2Percent = bMine.Item2Percent ?? bMineCommonSettings.Item2Percent;
            var bSectionChance = Random.Range(0, bItem1Percent + bItem2Percent);
            var bItemId = bSectionChance <= bItem1Percent
                ? pvpChest.SecondResourceFirstVariant
                : pvpChest.SecondResourceSecondVariant;
            var bRandomAmount = Random.Range(pvpChest.SecondResourceAmountMin, pvpChest.SecondResourceAmountMax + 1);
            pack.Add(new Item(bItemId, bRandomAmount));

            var cMine = GetMine(tierNumber, chestCommonSettings.CMineNumber - 1);
            var cMineCommonSettings = GetMineCommonSettings(tierNumber, chestCommonSettings.CMineNumber - 1);
            var cItem1Percent = cMine.Item2Percent ?? cMineCommonSettings.Item2Percent;
            var cItem2Percent = cMine.Item3Percent ?? cMineCommonSettings.Item3Percent;
            var cSectionChance = Random.Range(0, cItem1Percent + cItem2Percent);
            var cItemId = cSectionChance <= cItem1Percent
                ? pvpChest.ThirdResourceFirstVariant
                : pvpChest.ThirdResourceSecondVariant;
            var cRandomAmount = Random.Range(pvpChest.ThirdResourceAmountMin, pvpChest.ThirdResourceAmountMax + 1);
            pack.Add(new Item(cItemId, cRandomAmount));

            var shardCount = pvpChest.ShardAmount;
            var shard = GetShardRandomDrop();

            if(shard != "")
            pack.Add(new Item(shard, shardCount));

            return pack;
        }

        public static string GetShardRandomDrop()
        {
            var rand = Random.Range(1, 100);

            var shardDrops = App.Instance.StaticData.StaticDropChances.Where(x => x.Type == EntityType.Shard)
                .OrderByDescending(x=> x.Chance).Where(x => x.Chance != 0);


            foreach (var dropChance in shardDrops)
            {
                if (rand > dropChance.Chance)
                {
                    return dropChance.Id;
                }
            }

            return "";
        }

        public static long GetChestGoldAmount(int lvl, bool random = true)
        {
            var staticChest = App.Instance.StaticData.EnchantedChests[lvl];
            return random ? Random.Range(staticChest.GoldAmountMin, staticChest.GoldAmountMax + 1) : staticChest.GoldAmountMax;
        }

        public static Pack GetEnchantedChestRandomDrop(EnchantedChestType chestType, int level, int droppedArtefacts)
        {
            var pack = new Pack();
            var staticChest = App.Instance.StaticData.EnchantedChests[level];

            switch (chestType)
            {
                // gold
                case EnchantedChestType.Amber:
                    var lastTier = App.Instance.Player.Dungeon.LastOpenedTier.Number;
                    pack.Add(new Currency(CurrencyType.Gold, GetChestGoldAmount(lastTier,false)));
                    break;

                // resources
                case EnchantedChestType.Ruby:

                    var chestCommonSettings = GetChestCommonSettings(ChestType.Simple);
                    var tierNumber = chestCommonSettings.TierNumber - 1 + chestCommonSettings.TierOffset;
                    tierNumber = Mathf.Clamp(tierNumber, 0, App.Instance.StaticData.Tiers.Count);

                    var aMine = GetMine(tierNumber, chestCommonSettings.AMineNumber - 1);
                    var aMineCommonSettings = GetMineCommonSettings(tierNumber, chestCommonSettings.AMineNumber - 1);
                    var aItem1Percent = aMine.Item1Percent ?? aMineCommonSettings.Item1Percent;
                    var aItem2Percent = aMine.Item2Percent ?? aMineCommonSettings.Item2Percent;
                    var aSectionChance = Random.Range(0, aItem1Percent + aItem2Percent);
                    var aItemId = aSectionChance <= aItem1Percent ? staticChest.FirstResourceFirstVariant : staticChest.FirstResourceSecondVariant;
                    var aRandomAmount = Random.Range(staticChest.FirstResourceAmountMin, staticChest.FirstResourceAmountMax + 1);
                    pack.Add(new Item(aItemId, aRandomAmount));

                    var bMine = GetMine(tierNumber, chestCommonSettings.BMineNumber - 1);
                    var bMineCommonSettings = GetMineCommonSettings(tierNumber, chestCommonSettings.BMineNumber - 1);
                    var bItem1Percent = bMine.Item1Percent ?? bMineCommonSettings.Item1Percent;
                    var bItem2Percent = bMine.Item2Percent ?? bMineCommonSettings.Item2Percent;
                    var bSectionChance = Random.Range(0, bItem1Percent + bItem2Percent);
                    var bItemId = bSectionChance <= bItem1Percent ? staticChest.SecondResourceFirstVariant : staticChest.SecondResourceSecondVariant;
                    var bRandomAmount = Random.Range(staticChest.SecondResourceAmountMin, staticChest.SecondResourceAmountMax + 1);
                    pack.Add(new Item(bItemId, bRandomAmount));

                    var cMine = GetMine(tierNumber, chestCommonSettings.CMineNumber - 1);
                    var cMineCommonSettings = GetMineCommonSettings(tierNumber, chestCommonSettings.CMineNumber - 1);
                    var cItem1Percent = cMine.Item2Percent ?? cMineCommonSettings.Item2Percent;
                    var cItem2Percent = cMine.Item3Percent ?? cMineCommonSettings.Item3Percent;
                    var cSectionChance = Random.Range(0, cItem1Percent + cItem2Percent);
                    var cItemId = cSectionChance <= cItem1Percent ? staticChest.ThirdResourceFirstVariant : staticChest.ThirdResourceSecondVariant;
                    var cRandomAmount = Random.Range(staticChest.ThirdResourceAmountMin, staticChest.ThirdResourceAmountMax + 1);
                    pack.Add(new Item(cItemId, cRandomAmount));
                    break;

                // hilts
                case EnchantedChestType.Lazurite:

                    var hilts = App.Instance.StaticData.Hilts.Where(p => p.DropCategory == staticChest.HiltDropCategory).ToList();
                    if (hilts.Count > 0)
                    {
                        var randomValue = Random.Range(0, 100f);
                        Hilt droppedHilt = null;
                        var hiltChance = 0f;
                        foreach (var hilt in hilts)
                        {
                            hiltChance += hilt.DropChance;
                            if (randomValue > hiltChance)
                                continue;

                            droppedHilt = hilt;
                            break;
                        }

                        if (droppedHilt != null)
                            pack.Add(new Item(droppedHilt.Id, 1));
                    }
                    break;

                // items
                case EnchantedChestType.Malachite:

                    var randomItemIndex = Random.Range(0, 4);
                    string randomItemId;

                    switch (randomItemIndex)
                    {
                        case 0:
                            randomItemId = staticChest.FirstItemFirstVariant;
                            break;
                        case 1:
                            randomItemId = staticChest.FirstItemSecondVariant;
                            break;
                        case 2:
                            randomItemId = staticChest.FirstItemThirdVariant;
                            break;
                        case 3:
                            randomItemId = staticChest.FirstItemFourthVariant;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(randomItemIndex), randomItemIndex, null);
                    }

                    var randomItemAmount = Random.Range(staticChest.ItemAmountMin, staticChest.ItemAmountMax + 1);
                    pack.Add(new Item(randomItemId, randomItemAmount));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(chestType), chestType, null);
            }

            if (droppedArtefacts > 0)
                pack.Add(droppedArtefacts);

            return pack;
        }

        public static Pack GetGiftRandomDrop(int giftNumber)
        {
            var pack = new Pack();
            var staticChest = GetChest(ChestType.Simple, App.Instance.Player.Dungeon.LastOpenedTier.Number);
            var resourceCoefficient = App.Instance.StaticData.Configs.Gifts.SimpleResourcesCoefficient;

            if (giftNumber >= App.Instance.StaticData.Configs.Gifts.DailyCount - 1)
            {
                resourceCoefficient = App.Instance.StaticData.Configs.Gifts.RoyalResourcesCoefficient;
                var randomItemType = Random.Range(0f, 1f);
                if (randomItemType > 0.5f)
                {
                    var randomPotionIndex = Random.Range(0, App.Instance.StaticData.Potions.Count);
                    var randomPotion = App.Instance.StaticData.Potions[randomPotionIndex];
                    pack.Add(new Item(randomPotion.Id, App.Instance.StaticData.Configs.Gifts.RoyalRandomItemCount));
                }
                else
                {
                    var randomTntIndex = Random.Range(0, App.Instance.StaticData.Tnt.Count);
                    var randomTnt = App.Instance.StaticData.Tnt[randomTntIndex];
                    pack.Add(new Item(randomTnt.Id, App.Instance.StaticData.Configs.Gifts.RoyalRandomItemCount));
                }
            }

            var resourceId = staticChest.A1ItemId;
            var amountMin = staticChest.AAmountMin;
            var amountMax = staticChest.AAmountMax;

            var randomIndex = Random.Range(0, 6);
            switch (randomIndex)
            {
                case 1:
                    resourceId = staticChest.A2ItemId;
                    amountMin = staticChest.AAmountMin;
                    amountMax = staticChest.AAmountMax;
                    break;
                case 2:
                    resourceId = staticChest.B1ItemId;
                    amountMin = staticChest.BAmountMin;
                    amountMax = staticChest.BAmountMax;
                    break;
                case 3:
                    resourceId = staticChest.B2ItemId;
                    amountMin = staticChest.BAmountMin;
                    amountMax = staticChest.BAmountMax;
                    break;
                case 4:
                    resourceId = staticChest.C1ItemId;
                    amountMin = staticChest.CAmountMin;
                    amountMax = staticChest.CAmountMax;
                    break;
                case 5:
                    resourceId = staticChest.C2ItemId;
                    amountMin = staticChest.CAmountMin;
                    amountMax = staticChest.CAmountMax;
                    break;
            }

            var randomAmount = Random.Range(amountMin, amountMax + 1);
            randomAmount *= resourceCoefficient;

            pack.Add(new Item(resourceId, randomAmount));
            return pack;
        }


        public static SkillLevel GetSkillLevel(SkillType type, int level)
        {
            switch (type)
            {
                case SkillType.Damage:
                    return App.Instance.StaticData.DamageLevels.Count <= level
                        ? null
                        : App.Instance.StaticData.DamageLevels[level];
                case SkillType.Fortune:
                    return App.Instance.StaticData.FortuneLevels.Count <= level
                        ? null
                        : App.Instance.StaticData.FortuneLevels[level];
                case SkillType.Crit:
                    return App.Instance.StaticData.CritLevels.Count <= level
                        ? null
                        : App.Instance.StaticData.CritLevels[level];
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static AutoMinerSpeedLevels GetAutoMinerSpeedLevel(int lvl)
        {

            return App.Instance.StaticData.AutoMinerSpeedLevels.Count <= lvl
                ? null
                : App.Instance.StaticData.AutoMinerSpeedLevels[lvl];
        }

        public static AutoMinerCapacityLevels GetAutoMinerCapacityLevel(int lvl)
        {
            return App.Instance.StaticData.AutoMinerCapacityLevels.Count <= lvl
                ? null
                : App.Instance.StaticData.AutoMinerCapacityLevels[lvl];
        }

        public static AbilityLevel GetAbilityLevel(AbilityType type, int level)
        {
            switch (type)
            {
                case AbilityType.ExplosiveStrike:
                    return App.Instance.StaticData.ExplosiveStrikeLevels.Count <= level
                        ? null
                        : App.Instance.StaticData.ExplosiveStrikeLevels[level];
                case AbilityType.Freezing:
                    return App.Instance.StaticData.FreezingLevels.Count <= level
                        ? null
                        : App.Instance.StaticData.FreezingLevels[level];
                case AbilityType.Acid:
                    return App.Instance.StaticData.AcidLevels.Count <= level
                        ? null
                        : App.Instance.StaticData.AcidLevels[level];
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        public static Prestige GetCurrentPrestigeBuff()
        {
            return GetPrestigeBuff(App.Instance.Player.Prestige);
        }

        public static Prestige GetPrestigeBuff(int level)
        {
            return App.Instance.StaticData.Prestige[level - 1];
        }
    }
}