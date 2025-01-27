using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class Workshop
    {
        public List<Recipe> Recipes { get; }

        public List<WorkshopSlot> Slots { get; private set; }

        public List<WorkshopSlot> SlotsShard { get; private set; }

        public bool IsEmptyShardSlotExists => SlotsShard.Any(s => s.IsUnlocked && s.StaticRecipe == null);

        public bool IsEmptySlotExists => Slots.Any(s => s.IsUnlocked && s.StaticRecipe == null);

        public Buff MeltingBoost { get; private set; }

        public Buff ResourceBoost { get; private set; }

        public int MeltingCoefficient => (int) (MeltingBoost?.Value[BuffValueType.Melting] ?? 1);

        public int ResourceCoefficient => (int) (ResourceBoost?.Value[BuffValueType.Resource] ?? 1);


        public Workshop(CommonDLL.Dto.Workshop workshopResponse)
        {
            Recipes = new List<Recipe>();

            // ores
            var ores = App.Instance.StaticData.Recipes.Where(x => !x.Id.Contains("shard"));

            foreach (var staticRecipe in ores)
            {
                var foundResources = workshopResponse.Recipes?.Find(x => x.Id == staticRecipe.Id) != null
                    ? workshopResponse.Recipes.Find(x => x.Id == staticRecipe.Id).FoundResources
                    : new List<string>();

                var recipe = new Recipe(staticRecipe, new CommonDLL.Dto.Recipe(staticRecipe.Id, foundResources));
                Recipes.Add(recipe);
            }

            Slots = new List<WorkshopSlot>();

            for (var i = 0; i < App.Instance.StaticData.WorkshopSlots.Count; i++)
            {
                var staticSlot = App.Instance.StaticData.WorkshopSlots[i];

                var slot = workshopResponse.Slots != null && i < workshopResponse.Slots.Count
                    ? new WorkshopSlot(i, staticSlot, workshopResponse.Slots[i])
                    : new WorkshopSlot(i, staticSlot, staticSlot.PriceAmount <= 0);

                if (slot.StaticRecipe != null || i == 0)
                {
                    slot.UnLockSilence();
                }

                slot.SetType(WorkShopSlotType.Ore);
                Slots.Add(slot);
            }

            // shards
            var shards = App.Instance.StaticData.Recipes.Where(x => x.Id.Contains("shard"));

            foreach (var staticRecipe in shards)
            {
                var foundResources = workshopResponse.Recipes?.Find(x => x.Id == staticRecipe.Id) != null
                    ? workshopResponse.Recipes.Find(x => x.Id == staticRecipe.Id).FoundResources
                    : new List<string>();

                var recipe = new Recipe(staticRecipe, new CommonDLL.Dto.Recipe(staticRecipe.Id, foundResources));
                Recipes.Add(recipe);
            }

            SlotsShard = new List<WorkshopSlot>();

            for (var i = 0; i < App.Instance.StaticData.WorkshopSlots.Count; i++)
            {
                var staticSlot = App.Instance.StaticData.WorkshopSlots[i];
                var slot = workshopResponse.SlotsShard != null && i < workshopResponse.SlotsShard.Count
                    ? new WorkshopSlot(i, staticSlot, workshopResponse.SlotsShard[i])
                    : new WorkshopSlot(i, staticSlot, staticSlot.PriceAmount <= 0);

                if (slot.StaticRecipe != null || i == 0)
                {
                    slot.UnLockSilence();
                }

                slot.SetType(WorkShopSlotType.Shard);
                SlotsShard.Add(slot);
            }

            AddRestSlots();

            EventManager.Instance.Subscribe<EffectAddBuffEvent>(OnAddBuff);

        }

        private void AddRestSlots()
        {
            var staticWorkshopSlotsCount = App.Instance.StaticData.WorkshopSlots.Count;

            for (var i = 0; i < staticWorkshopSlotsCount; i++)
            {
                if (Slots.Count <= i)
                {
                    Slots.Add(new WorkshopSlot(i, App.Instance.StaticData.WorkshopSlots[i]));
                }
            }

            for (var i = 0; i < staticWorkshopSlotsCount; i++)
            {
                if (SlotsShard.Count <= i)
                {
                    SlotsShard.Add(new WorkshopSlot(i, App.Instance.StaticData.WorkshopSlots[i]));
                }
            }
        }

        private void OnAddBuff(EffectAddBuffEvent buff)
        {
            if (buff.Buff.StaticBuff.Type == BuffType.Boost)
            {
                // CHECK PRIORITY !
                if (buff.Buff.StaticBuff.Value.ContainsKey(BuffValueType.Melting))
                {
                    MeltingBoost = buff.Buff;
                    EventManager.Instance.Publish(new WorkshopBoostStartEvent(BuffValueType.Melting));
                }

                if (buff.Buff.StaticBuff.Value.ContainsKey(BuffValueType.Resource))
                {
                    ResourceBoost = buff.Buff;
                    EventManager.Instance.Publish(new WorkshopBoostStartEvent(BuffValueType.Melting));
                }

                EventManager.Instance.Subscribe<UnscaledSecondsTickEvent>(OnSecondTick);
            }
        }

        private void OnSecondTick(UnscaledSecondsTickEvent eventData)
        {
            CheckBoostedTime();
        }


        private void CheckBoostedTime()
        {
            if (ResourceBoost == null && MeltingBoost == null)
            {
                EventManager.Instance.Publish(new WorkshopBoostStopEvent());
                EventManager.Instance.Unsubscribe<UnscaledSecondsTickEvent>(OnSecondTick);
            }
        }

        public float GetMeltingBoostCoefficient(StaticData data)
        {

            var dateNow = TimeManager.Instance.NowUnixSeconds;

            if (App.Instance.Player.Effect.BuffList.Count == 0)
                return 1;

            CommonDLL.Static.Buff maxBuff = null;
            int maxPriority = -1;

            foreach (var buff in App.Instance.Player.Effect.BuffList)
            {
                var staticBuff = data.Buffs.Find(x => x.Id == buff.Id);
                if (staticBuff == null)
                    continue;


                if (buff.Time > dateNow)
                {
                    if (buff.Type == BuffType.Boost && staticBuff.Priority > maxPriority)
                    {
                        maxBuff = staticBuff;
                        maxPriority = staticBuff.Priority;
                    }
                }
            }

            if (maxBuff?.Value == null)
            {
                return 1;
            }
            else
            {
                if (maxBuff.Value.ContainsKey(BuffValueType.Melting))
                    return maxBuff.Value[BuffValueType.Melting];
            }

            return 1;
        }

        public List<string> GetAllUnlockedItems()
        {
            var list = new List<string>();
            foreach (var tir in Recipes)
            {
                foreach (var item in tir.FoundResources)
                {
                    list.Add(item);
                }
            }

            return list;
        }
    }
}