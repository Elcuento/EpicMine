using System;
using BlackTemple.Common;

using System.Collections.Generic;
using System.Linq;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine.Core
{
    public class Burglar
    {
        public List<Chest> Chests { get; }

        public bool IsFull => Chests.Count >= LocalConfigs.MaxChestCount;

        public Burglar(CommonDLL.Dto.Burglar burglarGameDataResponse)
        {
            Chests = new List<Chest>();

            if (burglarGameDataResponse.Chests != null)
            {
                foreach (var chest in burglarGameDataResponse.Chests)
                {
                    Chests.Add(new Chest(chest.Id, chest.Type, chest.Level, chest.StartBreakingTime));
                }
            }

            Subscribe();
        }



        public void AddChest(ChestType chestType, int level, Action<bool> onComplete = null)
        {
            if (Chests.Count >= LocalConfigs.MaxChestCount)
            {
                onComplete?.Invoke(false);
                return;
            }

            var chest = new Chest(Guid.NewGuid().ToString(), new Dto.Chest(chestType, level));
            Chests.Add(chest);
            onComplete?.Invoke(true);
            EventManager.Instance.Publish(new ChestAddedEvent(chest));
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<BurglarChestOpenedEvent>(OnChestOpened);
        }

        private void OnChestOpened(BurglarChestOpenedEvent eventData)
        {
            var chest = Chests.FirstOrDefault(ch => ch.Id == eventData.Chest.Id);
            Chests.Remove(chest);
        }

        public (long, long, long) OpenChestForcedInternal(Configs data, ChestType type, int cost, Chest chest = null)
        {

            if (cost > 0)
            {
                if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, cost))
                {
                    Debug.LogError("Dont have crystals");
                    return (0, 0, 0);
                }
            }

            var randomDroppedCrystalsAmount = GetChestRandomDroppedCrystals(data, type);
            if (randomDroppedCrystalsAmount > 0)
            {
                App.Instance.Player.Wallet.Add(CurrencyType.Crystals, randomDroppedCrystalsAmount, IncomeSourceType.FromMineChest);
            }

            var randomDroppedArtefactsAmount = GetChestRandomDroppedArtefacts(data, type);

            if (randomDroppedArtefactsAmount > 0)
                App.Instance.Player.Artefacts.Add(randomDroppedArtefactsAmount);

            if (chest != null)
            {
                App.Instance.Player.Burglar.Chests.Remove(chest);
            }


            return (cost,
                randomDroppedArtefactsAmount, randomDroppedCrystalsAmount);
        }

       public (long, long) OpenSpecificChestInternal(Configs data, Chest chest)
        {

            var randomDroppedCrystalsAmount = GetChestRandomDroppedCrystals(data, chest.Type);
            if (randomDroppedCrystalsAmount > 0)
            {
                App.Instance.Player.Wallet.Add(CurrencyType.Crystals, randomDroppedCrystalsAmount, IncomeSourceType.FromMineChest);
            }

            var randomDroppedArtefactsAmount = GetChestRandomDroppedArtefacts(data, chest.Type);

            if (randomDroppedArtefactsAmount > 0)
                App.Instance.Player.Artefacts.Add(randomDroppedArtefactsAmount);


            App.Instance.Player.Burglar.Chests.Remove(chest);


            return (randomDroppedArtefactsAmount, randomDroppedCrystalsAmount);
        }

       public (int, int) OpenChest(Configs data, ChestType type, Chest chest = null)
        {

            var randomDroppedCrystalsAmount = GetChestRandomDroppedCrystals(data, type);
            if (randomDroppedCrystalsAmount > 0)
            {
                App.Instance.Player.Wallet.Add(new Currency(CurrencyType.Crystals, randomDroppedCrystalsAmount), IncomeSourceType.FromMineChest);
            }

            var randomDroppedArtefactsAmount = GetChestRandomDroppedArtefacts(data, type);

            if (randomDroppedArtefactsAmount > 0)
                App.Instance.Player.Artefacts.Add(randomDroppedArtefactsAmount);

            if (chest != null)
            {
                App.Instance.Player.Burglar.Chests.Remove(chest);
            }

            return ((int)randomDroppedArtefactsAmount, randomDroppedCrystalsAmount);
        }

        int GetChestRandomDroppedCrystals(Configs data, ChestType type)
        {

            var simpleChestDropCrystalsData = data.Burglar.Chests[ChestType.Simple].Drop[ChestItemDropType.Crystals];
            var royalChestDropCrystalsData = data.Burglar.Chests[ChestType.Royal].Drop[ChestItemDropType.Crystals];
            var simpleChestDropCrystalsChance = simpleChestDropCrystalsData.Chance;
            var royalChestDropCrystalsChance = royalChestDropCrystalsData.Chance;
            var simpleChestDropCrystalsMin = simpleChestDropCrystalsData.Min;
            var royalChestDropCrystalsMin = royalChestDropCrystalsData.Min;
            var simpleChestDropCrystalsMax = simpleChestDropCrystalsData.Max;
            var royalChestDropCrystalsMax = royalChestDropCrystalsData.Max;

            var crystalsChance = type == ChestType.Royal
                ? royalChestDropCrystalsChance
                : simpleChestDropCrystalsChance;

            var randomCrystalsChance = Random.Range(0, 100);
            if (randomCrystalsChance > crystalsChance)
                return 0;

            var crystalsMin = type == ChestType.Royal
                ? royalChestDropCrystalsMin
                : simpleChestDropCrystalsMin;

            var crystalsMax = type == ChestType.Royal
                ? royalChestDropCrystalsMax
                : simpleChestDropCrystalsMax;

            return Random.Range((int)crystalsMin, (int)crystalsMax);

        }

        long GetChestRandomDroppedArtefacts(Configs data, ChestType type)
        {

            var maxArtefactsCount = data.Dungeon.TierOpenArtefactsCost;

            var userArtefacts = App.Instance.Player.Artefacts.Amount;
            maxArtefactsCount = maxArtefactsCount - userArtefacts;


            var simpleChestDropArtefactsData = data.Burglar.Chests[ChestType.Simple].Drop[ChestItemDropType.Artifacts];
            var royalChestDropArtefactsData = data.Burglar.Chests[ChestType.Royal].Drop[ChestItemDropType.Artifacts];
            var simpleChestDropArtefactsMin = simpleChestDropArtefactsData.Min;
            var royalChestDropArtefactsMin = royalChestDropArtefactsData.Min;
            var simpleChestDropArtefactsMax = simpleChestDropArtefactsData.Max;
            var royalChestDropArtefactsMax = royalChestDropArtefactsData.Max;

            var artefactsMin = type == ChestType.Royal
                ? royalChestDropArtefactsMin
                : simpleChestDropArtefactsMin;

            var artefactsMax = type == ChestType.Royal
                ? royalChestDropArtefactsMax
                : simpleChestDropArtefactsMax;

            if (artefactsMin > maxArtefactsCount)
                artefactsMin = maxArtefactsCount;

            if (artefactsMax > maxArtefactsCount)
                artefactsMax = maxArtefactsCount;

            return Random.Range((int)artefactsMin, (int)artefactsMax);

        }

        public (long, long, long) OpenChest(ChestType chestType)
        {
            var staticData = App.Instance.StaticData;

            long droppedCrystals = 0;
            long droppedArtefacts = 0;
            long cost = 0;

            var simpleChestBreakingTime = staticData.Configs.Burglar.Chests[ChestType.Simple].BreakingTimeInMinutes;
            var royalChestBreakingTime = staticData.Configs.Burglar.Chests[ChestType.Royal].BreakingTimeInMinutes;

            var chestForceCompletePricePer30Minutes =
                staticData.Configs.Burglar.ChestForceCompletePricePer30Minutes;

            var breakingMinutesLeft = chestType == ChestType.Royal
                ? royalChestBreakingTime
                : simpleChestBreakingTime;

            var forceOpenPrice = Math.Ceiling(breakingMinutesLeft / 30f) * chestForceCompletePricePer30Minutes;

            var res = OpenChestForcedInternal(staticData.Configs, chestType, (int)forceOpenPrice);

            droppedCrystals = res.Item3;
            droppedArtefacts = res.Item2;
            cost = res.Item1;

            var parameters = new CustomEventParameters
            {
                Int = new Dictionary<string, int>
                {
                    { "type", (int)chestType },
                    { "spent_crystals", (int)cost },
                    { "dropped_crystals", (int)droppedCrystals },
                    { "dropped_artefacts", (int)droppedArtefacts }
                }
            };

            App.Instance.Services.AnalyticsService.CustomEvent("force_open_chest", parameters);

            return res;
        }
    }
}