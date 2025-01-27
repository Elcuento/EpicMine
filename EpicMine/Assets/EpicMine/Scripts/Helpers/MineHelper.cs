using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using Exploder.Utils;
using UnityEngine;
using ColoredResource = BlackTemple.EpicMine.Dto.ColoredResource;
using Mine = BlackTemple.EpicMine.Core.Mine;
using Random = UnityEngine.Random;
using Tier = BlackTemple.EpicMine.Core.Tier;

namespace BlackTemple.EpicMine
{
    public static class MineHelper
    {
        public static ChestType GetRandomChestType()
        {
            var randomValue = Random.Range(0f, 100f);

            var logMessage = "Get random chest type, simple chest chance: {0}, random value: {1}";

            var simpleDropChance = App.Instance.StaticData.Configs.Burglar.Chests[ChestType.Simple].DropChance;
            var correctDropChance = App.Instance.Player.Burglar.IsFull ? simpleDropChance * 0.5 : simpleDropChance;
            App.Instance.Services.LogService.Log(string.Format(logMessage, correctDropChance, randomValue));


            return randomValue <= correctDropChance
                ? ChestType.Simple
                : ChestType.Royal;
        }

        public static EnchantedChestType GetRandomEnchantedChestType()
        {
            var randomValue = Random.Range(0f, 100f);

            var logMessage = "Get random chest type, random value: {0}";
            App.Instance.Services.LogService.Log(string.Format(logMessage, randomValue));

            if (randomValue <= App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Malachite].Chance)
                return EnchantedChestType.Malachite;

            if (randomValue <= App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Lazurite].Chance)
                return EnchantedChestType.Lazurite;

            if (randomValue <= App.Instance.StaticData.Configs.EnchantedChests.Chests[EnchantedChestType.Ruby].Chance)
                return EnchantedChestType.Ruby;

            return EnchantedChestType.Amber;
        }

        public static List<DropChance> GetRandomMonsterDropItems(Monster monster)
        {
            var drop = new List<DropChance>();

            if (monster.DropItems.Count <= 0)
                return drop;

            foreach (var dropChance in monster.DropItems)
            {
                if (dropChance.Chance >= Random.Range(0, 100))
                    drop.Add(dropChance);
            }

            return drop;
        }

        public static List<DropChance> GetRandomMonsterDropCurrency(Monster monster)
        {
            var drop = new List<DropChance>();

            if (monster.DropCurrency.Count <= 0)
                return drop;

            var random = Random.Range(0, 100);

            foreach (var dropChance in monster.DropCurrency)
            {
                if (dropChance.Chance >= random)
                    drop.Add(dropChance);
            }

            return drop;
        }


        public static int GetTierRequireArtefacts(int tier)
        {
            return tier >= App.Instance.StaticData.Tiers.Count ? App.Instance.StaticData.Tiers[App.Instance.StaticData.Tiers.Count - 1].RequireArtefacts : App.Instance.StaticData.Tiers[tier].RequireArtefacts;
        }

        public static string GetRandomWallDropItem(Core.Tier tier, Core.Mine mine)
        {
            var staticMine = StaticHelper.GetMine(tier.Number, mine.Number);
            var mineCommonSettings = StaticHelper.GetMineCommonSettings(tier.Number, mine.Number);

            var item1Chance = staticMine.Item1Percent ?? mineCommonSettings.Item1Percent;
            var item2Chance = staticMine.Item2Percent ?? mineCommonSettings.Item2Percent;
            var item3Chance = staticMine.Item3Percent ?? mineCommonSettings.Item3Percent;

            var randomValue = Random.Range(0f, item1Chance + item2Chance + item3Chance);
            string itemStaticId;

            var logMessage = "Get random wall drop item, item1 chance: {0}, item2 chance: {1}, item3 chance: {2}, random value: {3}";
            App.Instance.Services.LogService.Log(string.Format(logMessage, item1Chance, item2Chance, item3Chance, randomValue));

            if (randomValue <= item1Chance)
                itemStaticId = tier.StaticTier.WallItem1Id;
            else if (randomValue <= item1Chance + item2Chance)
                itemStaticId = tier.StaticTier.WallItem2Id;
            else
                itemStaticId = tier.StaticTier.WallItem3Id;

            return itemStaticId;
        }

        public static ColoredResource GetColoredResource(string resourceId, Core.Tier tier)
        {
            var color = "#ffffff";

            if (resourceId == tier.StaticTier.WallItem2Id)
                color = "#2db8ff";
            else if (resourceId == tier.StaticTier.WallItem3Id)
                color = "#ff742d";

            return new ColoredResource(resourceId, color);
        }

        public static bool GetRandomChestSectionValue(int tierNumber, int mineNumber)
        {
            var lastDroppedTime = App.Instance
                .Services
                .RuntimeStorage
                .Load<DateTime>(RuntimeStorageKeys.LastChestDroppedTime);

            if (lastDroppedTime <= DateTime.MinValue)
                lastDroppedTime = DateTime.Now;

            var staticTier = StaticHelper.GetTier(tierNumber);
            var staticMine = StaticHelper.GetMine(tierNumber, mineNumber);
            var mineCommonSettings = StaticHelper.GetMineCommonSettings(tierNumber, mineNumber);

            var chestChance = staticMine.ChestPercent ?? mineCommonSettings.ChestPercent;
            var timeSinceLastChestDropped = DateTime.Now - lastDroppedTime;
            if (timeSinceLastChestDropped.TotalMinutes >= staticTier.SecondIncreaseChestProbabilityTime)
                chestChance += staticTier.SecondIncreaseChestProbabilityPercent;
            else if (timeSinceLastChestDropped.TotalMinutes >= staticTier.FirstIncreaseChestProbabilityTime)
                chestChance += staticTier.FirstIncreaseChestProbabilityPercent;

            var randomValue = Random.Range(0f, 100f);
            App.Instance.Services.LogService.Log($"Get random mine section, chest section chance: {chestChance}, random value: {randomValue}");

            if (randomValue > chestChance)
                return false;

            App.Instance
                .Services
                .RuntimeStorage
                .Save(RuntimeStorageKeys.LastChestDroppedTime, DateTime.Now);

            return true;
        }

        public static float GetCurrentPickaxeDamage()
        {
            var staticPickaxe = App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe;
            var baseDamage = App.Instance.Player.Skills.Damage.Value;
            var damage = staticPickaxe.Damage + baseDamage;

            var damageLogMessage = "pickaxe damage: {0}, pickaxe bonus damage coefficient: {1}";
         //   App.Instance.Services.LogService.Log(string.Format(damageLogMessage, staticPickaxe.Damage, staticPickaxe.BonusDamagePercent));

            if (staticPickaxe.BonusDamagePercent > 0)
                damage += damage * (staticPickaxe.BonusDamagePercent.Value / 100);

            return damage;
        }


        public static void AddDroppedItem(Item item)
        {
            var lastMineDroppedItems = App.Instance
                                           .Services
                                           .RuntimeStorage
                                           .Load<Dictionary<string, int>>(RuntimeStorageKeys.LastMineDroppedItems) ??
                                       new Dictionary<string, int>();

            lastMineDroppedItems.TryGetValue(item.Id, out var newAmount);
            newAmount += item.Amount;
            lastMineDroppedItems[item.Id] = newAmount;

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.LastMineDroppedItems, lastMineDroppedItems);
        }

        public static void UseAbility(AbilityType abilityType)
        {
            var usedAbilities = App.Instance
                                    .Services
                                    .RuntimeStorage
                                    .Load<Dictionary<AbilityType, int>>(RuntimeStorageKeys.UsedAbilities) ??
                                new Dictionary<AbilityType, int>();

            usedAbilities.TryGetValue(abilityType, out var newAmount);
            newAmount++;
            usedAbilities[abilityType] = newAmount;

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.UsedAbilities, usedAbilities);
        }

        /*public static void UseItem(BuffType buffType)
        {
            var usedItems = App.Instance
                                .Services
                                .RuntimeStorage
                                .Load<Dictionary<BuffType, int>>(RuntimeStorageKeys.UsedItems) ??
                            new Dictionary<BuffType, int>();

            int newAmount;
            usedItems.TryGetValue(buffType, out newAmount);
            newAmount++;
            usedItems[buffType] = newAmount;

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.UsedItems, usedItems);
        }*/

        public static int GetBossDropChance(TierBoss boss)
        {
            return Random.Range(boss.AmountMin, boss.AmountMax + 1);
        }

        public static void AddDroppedCurrency(Dto.Currency currency)
        {
            var lastMineDroppedCurrencies = App.Instance
                                                .Services
                                                .RuntimeStorage
                                                .Load<Dictionary<CurrencyType, int>>(RuntimeStorageKeys
                                                    .LastMineDroppedCurrencies) ?? new Dictionary<CurrencyType, int>();

            lastMineDroppedCurrencies.TryGetValue(currency.Type, out var newAmount);
            newAmount += (int)currency.Amount;
            lastMineDroppedCurrencies[currency.Type] = newAmount;

            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.LastMineDroppedCurrencies, lastMineDroppedCurrencies);
        }

        public static Ghost GetTierGhost(Tier tier, Mine mine)
        {
        
            if (mine.IsGhostAppear) return null;

            var ghost = App.Instance.StaticData.Ghosts.Find(x => x.Tier == tier.Number + 1);

            if (ghost == null) return null;

            if (tier.GhostActionsCount < ghost.Actions.Count)
            {
                var ghostsLeft = ghost.Actions.Count - tier.GhostActionsCount;
                var tierTotalMines = tier.Mines.Count;

                if (tierTotalMines == mine.Number + 1)
                    return null;

                if (tierTotalMines - (mine.Number + 1) <= ghostsLeft)
                {
                    return ghost;
                }

                if (Random.Range(0, 100) > MineLocalConfigs.MineGhostAppearChance)
                {
                    return ghost;
                }
            }

            return null;
        }

        public static void ClearTempStorage(bool withClearSelectedMine = true)
        {
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.LastMineDroppedItems);
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.LastMineDroppedCurrencies);
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.UsedAbilities);
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.UsedItems);
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.Energy);
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.HealthRefillCount);
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.FoundChestCount);
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.IsMissedAtLeastOnce);
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.TierGhost);

            if (withClearSelectedMine)
            {
                App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.SelectedMine);
                App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.SelectedTier);
            }
        }

        public static void AddStartMiningEventToAnalytics(bool isAlreadyCompleted = false)
        {
            var selectedMine = App.Instance.Services.RuntimeStorage.Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);
            var selectedTier = App.Instance.Services.RuntimeStorage.Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            App.Instance.Services.AnalyticsService.StartMining(selectedTier.Number, selectedMine.Number, isAlreadyCompleted);
        }

        public static void AddEndMiningEventToAnalytics(bool isComplete = false, bool isAlreadyCompleted = false)
        {
            var earned = new Dictionary<string, int>();
            var spent = new Dictionary<string, int>();

            var lastMineDroppedItems = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<string, int>>(RuntimeStorageKeys.LastMineDroppedItems);

            var lastMineDroppedCurrencies = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<CurrencyType, int>>(RuntimeStorageKeys.LastMineDroppedCurrencies);

            var foundChestCount = App.Instance
                .Services
                .RuntimeStorage
                .Load<int>(RuntimeStorageKeys.FoundChestCount);

            var healthRefillCount = App.Instance
                .Services
                .RuntimeStorage
                .Load<int>(RuntimeStorageKeys.HealthRefillCount);

            /*var usedItems = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<BuffType, int>>(RuntimeStorageKeys.UsedItems);*/

            var usedAbilities = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<AbilityType, int>>(RuntimeStorageKeys.UsedAbilities);

            var selectedMine = App.Instance.Services.RuntimeStorage.Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);
            var selectedTier = App.Instance.Services.RuntimeStorage.Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            if (lastMineDroppedCurrencies != null)
            {
                foreach (var currency in lastMineDroppedCurrencies)
                    earned.Add(currency.Key.ToString(), currency.Value);
            }

            if (lastMineDroppedItems != null)
            {
                foreach (var item in lastMineDroppedItems)
                    earned.Add(item.Key, item.Value);
            }

            if (foundChestCount > 0)
                spent.Add("FoundChestsCount", foundChestCount);

            if (healthRefillCount > 0)
                spent.Add("RefillPickaxeHealth", healthRefillCount);

            if (usedAbilities != null)
            {
                foreach (var ability in usedAbilities)
                    spent.Add(ability.Key.ToString(), ability.Value);
            }

            /*if (usedItems != null)
            {
                foreach (var item in usedItems)
                    spent.Add(item.Key.ToString(), item.Value);
            }*/

            App.Instance
                .Services
                .AnalyticsService
                .EndMining(
                    selectedTier.Number,
                    selectedMine.Number,
                    isComplete,
                    earned,
                    spent,
                    isAlreadyCompleted);
        }

        public static int[] GetGarantPlace(MineSceneAttackPoint[,] pointMatrix, Core.FieldFigure figure)
        {
            var dem1 = pointMatrix.GetLength(0);
            var dem2 = pointMatrix.GetLength(1);

            var figSize1 = figure.GetMaxXPoint();
            var figSize2 = figure.GetMaxYPoint();

            for (var startX = 0; startX < dem1 - figSize1; startX += 2)
            {
                for (var startY = 0; startY < dem2 - figSize2; startY += 2)
                {
                    for (var i = startX; i < dem1; i += 2)
                    {
                        for (var j = startY; j < dem2; j += 2)
                        {
                            if (pointMatrix[i, j] == null)
                            {
                                var isOk = true;

                                var x = 0;
                                for (var a = i; a < i + figSize1; a += 2)
                                {
                                    var y = -1;
                                    for (var b = j; b < j + figSize2; b += 2)
                                    {
                                        y++;

                                        if (a >= dem1 - figure.Grid[x, y].Size || b >= dem2 - figure.Grid[x, y].Size || pointMatrix[a, b] != null)
                                        {
                                            isOk = false;
                                            break;
                                        }

                                        if (!CheckNeighborhoods(pointMatrix, a, b, figure.Grid[x, y].Size))
                                        {
                                            isOk = false;
                                            break;
                                        }
                                    }

                                    if (!isOk)
                                        break;
                                    x++;
                                }

                                if (isOk)
                                {
                                    return new[] {i, j};
                                }
                            }
                        }
                    }
                }
            }

            return new int[0];
        }

        public static int[] GetPlace(MineSceneAttackPoint[,] pointMatrix, Core.FieldFigure figure)
        {
            var dem1 = pointMatrix.GetLength(0);
            var dem2 = pointMatrix.GetLength(1);
            var figSize1 = figure.GetMaxXPoint();
            var figSize2 = figure.GetMaxYPoint();

            var startX = new System.Random(DateTime.Now.Millisecond).Next(0, dem1 - figSize1);
            var startY = new System.Random(DateTime.Now.Millisecond).Next(0, dem2 - figSize2);


            for (var i = startX; i < dem1; i += 2)
            {
                for (var j = startY; j < dem2; j += 2)
                {
                    if (pointMatrix[i, j] == null)
                    {
                        var isOk = true;

                        var x = 0;
                        for (var a = i; a < i + figSize1; a += 2)
                        {
                            var y = -1;
                            for (var b = j; b < j + figSize2; b += 2)
                            {
                                y++;

                                if (a >= dem1 - 2 || b >= dem2 - 2 || pointMatrix[a, b] != null || x >= dem1 ||
                                    y >= dem2)
                                {
                                    isOk = false;
                                    break;
                                }

                                if (!CheckNeighborhoods(pointMatrix, a, b, figure.Grid[x, y].Size))
                                {
                                    isOk = false;
                                    break;
                                }


                            }

                            if (!isOk)
                                break;
                            x++;
                        }

                        if (isOk)
                        {
                            return new[] { i, j };
                        }
                    }
                }
            }

            return new int[0];
        }

       /* public static int[] GetPlace(MineSceneAttackPoint[,] pointMatrix, Core.FieldFigure figure, int startX, int startY)
        {
            var dem1 = pointMatrix.GetLength(0);
            var dem2 = pointMatrix.GetLength(1);

            var figSize1 = figure.LengthX * 2;
            var figSize2 = figure.LengthY * 2;

            startX += 1;
            startY += 1;


            for (var i = startX; i < dem1; i += 2)
            {
                for (var j = startY; j < dem2; j += 2)
                {
                    if (pointMatrix[i, j] == null)
                    {
                        var isOk = true;

                        var x = 0;
                        for (var a = i; a < i + figSize1; a += 2)
                        {
                            var y = -1;
                            for (var b = j; b < j + figSize2; b += 2)
                            {
                                y++;

                                if (figure.Grid[x, y].PointType == AttackPointType.Empty)
                                    continue;

                                if (a >= dem1 - 2 || b >= dem2 - 2 || pointMatrix[a, b] != null)
                                {
                                    isOk = false;
                                    break;
                                }

                                if (!CheckNeighborhoods(pointMatrix, a, b))
                                {
                                    isOk = false;
                                    break;
                                }
                            }

                            if (!isOk)
                                break;
                            x++;
                        }

                        if (isOk)
                        {
                            return new[] { i, j };
                        }
                    }
                }
            }

            return new int[0];
        }*/

        public static bool CheckNeighborhoods(MineSceneAttackPoint[,] pointMatrix, int x, int y, int size)
        {
            var gridSizeX = pointMatrix.GetLength(0);
            var gridSizeY = pointMatrix.GetLength(1);

            for (var i = -size; i <= size; i++)
            {
                for (var j = -size; j <= size; j++)
                {
                    var negX = x + i;
                    var negY = y + j;

                    if (negX >= 0 && negY >= 0 && negX < gridSizeX && negY < gridSizeY)
                    {
                        if (pointMatrix[negX, negY] != null)
                            return false;
                    }
                }
            }

            return true;
        }

        public static Core.FieldFigure GetRandomFigure(List<Core.FieldFigure> staticFigures, Core.FieldPointArea area, int maxPoints)
        {
       
            var areaFigureChances = area.Figures;
            Core.FieldFigure figure = null;

            var figureChances = new Dictionary<Core.FieldFigure, float>();

            var figChance = areaFigureChances.FirstOrDefault(x => x.Value > 0);
            var totalChance = areaFigureChances.Values.Sum(x => x);

            if (figChance.Value == 0)
            {
                return staticFigures.Find(x => x.Id == MineLocalConfigs.PointFigure);
            }

            foreach (var figureStatic in staticFigures)
            {
                if (figureStatic.GetPointsCount() < maxPoints)
                {
                    areaFigureChances.TryGetValue(figureStatic.Id, out var spawnPercent);

                    var spawnChance = (spawnPercent / totalChance) * 100f;

                    if(spawnChance != 0)
                    figureChances.Add(figureStatic, spawnPercent);
                }
                
            }

            var sorted = figureChances.OrderBy(x => x.Value).ToList();

            Core.FieldFigure resultFigure = null;
            var resultFigureChance = 0f;

            var chance = new System.Random(DateTime.Now.Millisecond).Next(1, 100);

            foreach (var attackPointFigure in sorted)
            {
                if (chance < attackPointFigure.Value)
                {
                    var allFigureSameChance = sorted.Where(x => x.Value == chance).ToList();
                    if (allFigureSameChance.Count > 0)
                    {
                        var random = new System.Random(DateTime.Now.Millisecond).Next(0, sorted.Count);

                        return sorted[random].Key;
                    }
                    return attackPointFigure.Key;
                }
            }

            return staticFigures.Find(x => x.Id == MineLocalConfigs.PointFigure);
        }
    }
}