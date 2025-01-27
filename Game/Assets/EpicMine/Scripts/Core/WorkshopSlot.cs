using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;
using Debug = UnityEngine.Debug;

namespace BlackTemple.EpicMine.Core
{
    public class WorkshopSlot
    {
        public int Number { get; }

        public CommonDLL.Static.WorkshopSlot StaticSlot { get; }

        public WorkShopSlotType SlotType { get; private set; }

        public bool IsUnlocked { get; private set; }

        public CommonDLL.Static.Recipe StaticRecipe { get; private set; }

        public int CompleteAmount { get; private set; }

        public int NecessaryAmount { get; private set; }

        public DateTime? MeltingStartTime { get; private set; }

        public bool IsComplete => IsUnlocked && StaticRecipe != null && CompleteAmount == NecessaryAmount;

        public bool IsDestroyed;

        public void SetType(WorkShopSlotType type)
        {
            SlotType = type;
        }
        public TimeSpan TimeLeft
        {
            get
            {
                var timeCoefficient = App.Instance.Player.Workshop.MeltingCoefficient;

                return MeltingStartTime.Value.AddSeconds(StaticRecipe.CraftTime / timeCoefficient) - TimeManager.Instance.Now;
            }
        }

        public TimeSpan FullAmountTimeLeft
        {
            get
            {
                var amountLeft = NecessaryAmount - CompleteAmount;
                if (amountLeft <= 1)
                    return TimeLeft;

                var timePassed = RecipeCraftTime - TimeLeft;
                var allCraftSeconds = amountLeft * RecipeCraftTime.TotalSeconds;
                return TimeSpan.FromSeconds(allCraftSeconds - timePassed.TotalSeconds);
            }
        }

        public TimeSpan RecipeCraftTime
        {
            get
            {
                var timeCoefficient = App.Instance.Player.Workshop.MeltingCoefficient;
                return TimeSpan.FromSeconds(StaticRecipe.CraftTime / timeCoefficient);
            }
        }

        public int ForceCompletePrice
        {
            get
            {
                if (IsComplete)
                    return 0;

                var forceCompleteMinutesLeft = Mathf.CeilToInt((float)FullAmountTimeLeft.TotalSeconds / 60);
                if (forceCompleteMinutesLeft <= 1)
                    return App.Instance.StaticData.Configs.Workshop.ForceCompletePrices.LessThanOneMinute;

                if (forceCompleteMinutesLeft <= 5)
                    return App.Instance.StaticData.Configs.Workshop.ForceCompletePrices.LessThanFiveMinutes;

                if (forceCompleteMinutesLeft <= 10)
                    return App.Instance.StaticData.Configs.Workshop.ForceCompletePrices.LessThanTenMinutes;

                var price = App.Instance.StaticData.Configs.Workshop.ForceCompletePrices.LessThanTenMinutes;
                var numberOfTen = Mathf.CeilToInt((forceCompleteMinutesLeft - 10) / 10f);

                price += numberOfTen * App.Instance.StaticData.Configs.Workshop.ForceCompletePrices.MoreThanTenMinutesForEveryTen;

                return price;
            }
        }


        public WorkshopSlot(int number, CommonDLL.Static.WorkshopSlot staticSlot, bool isUnlocked = false)
        {
            Number = number;
            StaticSlot = staticSlot;
            IsUnlocked = isUnlocked;
        }

        public WorkshopSlot(int number, CommonDLL.Static.WorkshopSlot staticSlot, CommonDLL.Dto.WorkshopSlot dtoSlot)
        {
            Number = number;
            StaticSlot = staticSlot;

            IsUnlocked = dtoSlot.IsUnlocked;
            NecessaryAmount = dtoSlot.NecessaryAmount;

            if (dtoSlot.MeltingStartTime.HasValue)
                MeltingStartTime = dtoSlot.MeltingStartTime.Value;

            if (!string.IsNullOrEmpty(dtoSlot.ItemId))
            {
                StaticRecipe = App.Instance.StaticData.Recipes.FirstOrDefault(r => r.Id == dtoSlot.ItemId &&
                                                                                   r.Type == dtoSlot.Type);
            }

            if (IsUnlocked == false || StaticRecipe == null || CompleteAmount == NecessaryAmount)
                return;
        }

        public void Destroy()
        {
            Clear(true);
        }

        public void UnLockSilence()
        {
            IsUnlocked = true;
        }
        public void Unlock()
        {
            if (IsDestroyed)
                return;

            var priceCurrency = new Currency(StaticSlot.PriceCurrencyType, StaticSlot.PriceAmount);

            if (!App.Instance.Player.Wallet.Has(priceCurrency))
                return;


            var staticData = App.Instance.StaticData;

            var staticWorkshopSlotsCount = staticData.WorkshopSlots.Count;
            if (staticWorkshopSlotsCount <= Number)
            {
                UnityEngine.Debug.LogError("Slots already more than static number");
                return ;
            }

            if (SlotType == WorkShopSlotType.Ore)
            {
               /* if (App.Instance.Player.Workshop.Slots.Count > Number)
                {
                    UnityEngine.Debug.LogError("Slots already more than number");
                    return ;
                }*/

                if (staticData.WorkshopSlots[Number].PriceCurrencyType == CurrencyType.Crystals)
                {
                    if (!App.Instance.Player.Wallet.Has(CurrencyType.Crystals,
                        staticData.WorkshopSlots[Number].PriceAmount))
                    {
                        UnityEngine.Debug.LogError("Not enough crystals");
                        return;
                    }

                    if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals,
                        staticData.WorkshopSlots[Number].PriceAmount))
                    {
                        UnityEngine.Debug.LogError("Not enough crystals");
                        return;
                    }
                }


                while (App.Instance.Player.Workshop.Slots.Count <= Number)
                {
                    App.Instance.Player.Workshop.Slots[^1]
                        .IsUnlocked = true;
                }
            }
            else if (SlotType == WorkShopSlotType.Shard)
            {
           /*     if (App.Instance.Player.Workshop.SlotsShard.Count > Number)
                {
                    UnityEngine.Debug.LogError("Slots shard already more than number");
                    return;
                }
                */
                if (staticData.WorkshopSlots[Number].PriceCurrencyType == CurrencyType.Crystals)
                {
                    if (!App.Instance.Player.Wallet.Has(CurrencyType.Crystals,
                        staticData.WorkshopSlots[Number].PriceAmount))
                    {
                        UnityEngine.Debug.LogError("Currency not exist for slots shard");
                        return;
                    }

                    if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals,
                        staticData.WorkshopSlots[Number].PriceAmount))
                    {
                        UnityEngine.Debug.LogError("Not enough crystals for slots shards");
                        return;
                    }
                }

                while (App.Instance.Player.Workshop.SlotsShard.Count <= Number)
                {
                    App.Instance.Player.Workshop.SlotsShard[^1]
                        .IsUnlocked = true;
                }

            }

            Debug.Log("Unlock"+ Number);
            IsUnlocked = true;


            var parameters = new CustomEventParameters
            {
                Int = new Dictionary<string, int>
                {
                    { "price_amount", (int)priceCurrency.Amount },
                },
                String = new Dictionary<string, string>
                {
                    { "price_currency", priceCurrency.Type.ToString() }
                }
            };
            App.Instance.Services.AnalyticsService.CustomEvent("unlock_workshop_slot", parameters);

            var unlockEvent = new WorkshopSlotUnlockEvent(this);
            EventManager.Instance.Publish(unlockEvent);
        }

        public void Start(Recipe recipe, int necessaryAmount)
        {
            if (IsDestroyed || recipe == null)
            {
                if (StaticRecipe != null)
                {
                    UnityEngine.Debug.LogError("is busy");
                }
                return;
            }

            var ingredients = StaticHelper.GetIngredients(recipe.StaticRecipe, necessaryAmount);

            if (!App.Instance.Player.Inventory.Has(ingredients))
            {
                return;
            }


            App.Instance.Player.Inventory.Remove(ingredients, SpendType.Using);

            StaticRecipe = recipe.StaticRecipe;
            NecessaryAmount = necessaryAmount;
            MeltingStartTime = TimeManager.Instance.Now;

            var startEvent = new WorkshopSlotStartMeltingEvent(this);

            EventManager.Instance.Publish(startEvent);

            EventManager.Instance.Subscribe<UnscaledSecondsTickEvent>(OnSecondTick);

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.WorkshopSlotSetRecipe);
        }

        public void CollectCompleted(int amount = 0, bool adsShowed = false, bool crystalPayed = false)
        {
            if (IsDestroyed)
                return;

            var boost = App.Instance.Player.Workshop.ResourceCoefficient;

            var isDouble = adsShowed || crystalPayed;

            if (CompleteAmount <= 0)
                return;

            var amountToCollect = 0;
            if (amount > 0)
                amountToCollect = amount;

            if (amount > CompleteAmount)
                amountToCollect = CompleteAmount;


            var collectedAmount = (isDouble ? amountToCollect * 2 : amountToCollect) * boost;

            if (isDouble)
            {
                var parameters = new CustomEventParameters
                {
                    Int = new Dictionary<string, int>
                    {
                        {"spent_crystals", crystalPayed ? App.Instance.StaticData.Configs.Workshop.CollectCrystalPrice : 0 },
                    }
                };
                App.Instance.Services.AnalyticsService.CustomEvent("complete_workshop_slot_double", parameters);
            }
            else
            {
                App.Instance.Services.AnalyticsService.CustomEvent("complete_workshop_slot", new CustomEventParameters());
            }


            App.Instance.Player.Inventory.Add(
                new Item(StaticRecipe.Id, collectedAmount * StaticRecipe.Amount),
                IncomeSourceType.FromCraft);
            if (amountToCollect == NecessaryAmount)
                Clear();
            else
            {
                NecessaryAmount -= amountToCollect;
                CompleteAmount -= amountToCollect;
                OnChange();
            }
        }

        public void Stop()
        {
            if (IsDestroyed)
                return;

            if (CompleteAmount >= NecessaryAmount)
                return;

            var ingredients = StaticHelper.GetIngredients(StaticRecipe, NecessaryAmount - CompleteAmount);
            App.Instance.Player.Inventory.Add(ingredients, IncomeSourceType.FromStopCraft);
            App.Instance.Player.Inventory.Add(new Item(StaticRecipe.Id, CompleteAmount * StaticRecipe.Amount), IncomeSourceType.FromCraft);
            Clear();
        }

        public void ForceComplete(bool isTutorial = false)
        {
            if (IsDestroyed)
                return;

            if (CompleteAmount == NecessaryAmount)
                return;

            if (!isTutorial)
            {
                var cost = new Currency(CurrencyType.Crystals, ForceCompletePrice);

                if (!App.Instance.Player.Wallet.Has(cost))
                {
                    WindowManager.Instance.Show<WindowShop>()
                        .OpenCrystals();

                    return;
                }
            }

            var staticData = App.Instance.StaticData;

            if (isTutorial)
            {

                if (StaticRecipe == null)
                {
                    UnityEngine.Debug.LogError("Error recipe data");
                    return;
                }

                var timeSpend = (StaticRecipe.CraftTime / App.Instance.Player.Workshop.GetMeltingBoostCoefficient(staticData) *
                                 NecessaryAmount);

                var now = TimeManager.Instance.NowUnixSeconds;

                MeltingStartTime =
                   TimeManager.Instance.FromUnixToDateTime((now - (long)Math.Floor(timeSpend)));

                App.Instance.Player.Wallet.Remove(new Currency(CurrencyType.Crystals, ForceCompletePrice));

                var parameters = new CustomEventParameters
                {
                    Int = new Dictionary<string, int>
                    {
                        {"spent_crystals", ForceCompletePrice},
                    }
                };
                App.Instance.Services.AnalyticsService.CustomEvent("force_complete_workshop_slot", parameters);

                Complete();
            }
            else
            {

                var meltingStartTime = TimeManager.Instance.ToDateTimeOffset(MeltingStartTime ?? DateTime.Now).ToUnixTimeSeconds();

                // var craftEndTime = meltingStartTime + (recipeData.CraftTime  * necessaryAmount);
                var now = TimeManager.Instance.NowUnixSeconds;

                var timeSpend = (StaticRecipe.CraftTime / App.Instance.Player.Workshop.GetMeltingBoostCoefficient(staticData) *
                                 NecessaryAmount);

                var craftEndTime = Math.Floor(meltingStartTime + timeSpend);

                if (now > craftEndTime)
                {
                    Debug.LogError("Not ready");
                    return;
                }

                var minutesLeft = Math.Ceiling(((double)craftEndTime - now) / (60));
                var cost = 0;

                if (minutesLeft <= 1)
                    cost = staticData.Configs.Workshop.ForceCompletePrices.LessThanOneMinute;
                else if (minutesLeft <= 5)
                    cost = staticData.Configs.Workshop.ForceCompletePrices.LessThanFiveMinutes;
                else if (minutesLeft <= 10)
                    cost = staticData.Configs.Workshop.ForceCompletePrices.LessThanTenMinutes;
                else
                {
                    var numberOfTen = Math.Ceiling((minutesLeft - 10) / 10);
                    cost = (int)(staticData.Configs.Workshop.ForceCompletePrices.LessThanTenMinutes + numberOfTen *
                        staticData.Configs.Workshop.ForceCompletePrices.MoreThanTenMinutesForEveryTen);
                }

                if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, cost))
                {
                    UnityEngine.Debug.LogError("Not enough crystals");
                    return;
                }

                MeltingStartTime =
                    TimeManager.Instance.FromUnixToDateTime((now - (long)Math.Floor(timeSpend)));

                var parameters = new CustomEventParameters
                {
                    Int = new Dictionary<string, int>
                    {
                        {"spent_crystals", cost},
                    }
                };
                App.Instance.Services.AnalyticsService.CustomEvent("force_complete_workshop_slot", parameters);

                Complete();
            }
        }

        public void CalculateCompleted()
        {
            if (IsDestroyed)
                return;

            if (!IsUnlocked || MeltingStartTime == null || StaticRecipe == null)
                return;

            var timeCoefficient = 1;

            var buff = App.Instance.Player.Effect.GetBuff(BuffType.Boost, BuffValueType.Melting);
            if (buff != null)
                timeCoefficient = (int)buff.Value[BuffValueType.Melting];
            

            for (var i = CompleteAmount; i < NecessaryAmount; i++)
            {
                var endTime = MeltingStartTime.Value.AddSeconds(StaticRecipe.CraftTime / timeCoefficient);
                if (DateTime.UtcNow > endTime)
                {
                    CompleteAmount++;
                    MeltingStartTime = endTime;
                }
                else
                {
                    EventManager.Instance.Unsubscribe<UnscaledSecondsTickEvent>(OnSecondTick);
                    EventManager.Instance.Subscribe<UnscaledSecondsTickEvent>(OnSecondTick);
                    return;
                }
            }

            if (CompleteAmount >= NecessaryAmount)
            {
                CompleteAmount = NecessaryAmount;
                MeltingStartTime = DateTime.MinValue;
                EventManager.Instance.Unsubscribe<UnscaledSecondsTickEvent>(OnSecondTick);
            }
        }


        private void OnSecondTick(UnscaledSecondsTickEvent eventData)
        {
            if (IsDestroyed)
                return;

            if (TimeLeft.TotalSeconds > 0)
            {
                EventManager.Instance.Publish(new WorkshopSlotMeltingTimeLeftChangeEvent(this));
                return;
            }

            CompleteAmount++;
            MeltingStartTime = TimeManager.Instance.Now;
            OnChange();

            if (CompleteAmount >= NecessaryAmount)
                Complete();
        }

        private void OnChange()
        {
            var changeEvent = new WorkshopSlotChangeEvent(this);
            EventManager.Instance.Publish(changeEvent);
        }


        private void Clear(bool destroy = false)
        {
            StaticRecipe = null;
            CompleteAmount = 0;
            NecessaryAmount = 0;
            MeltingStartTime = DateTime.MinValue;
            EventManager.Instance.Unsubscribe<UnscaledSecondsTickEvent>(OnSecondTick);

            IsDestroyed = destroy;

            var clearEvent = new WorkshopSlotClearEvent(this);
            EventManager.Instance.Publish(clearEvent);
        }

        private void Complete()
        {
            if (IsDestroyed)
                return;

            CompleteAmount = NecessaryAmount;
            MeltingStartTime = DateTime.MinValue;
            EventManager.Instance.Unsubscribe<UnscaledSecondsTickEvent>(OnSecondTick);

            var workshopSlotCompleteEvent = new WorkshopSlotCompleteEvent(this);
            EventManager.Instance.Publish(workshopSlotCompleteEvent);
        }
    }
}