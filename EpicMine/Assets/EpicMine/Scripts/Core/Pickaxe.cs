using System;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class Pickaxe
    {
        public bool IsCreated { get; private set; }

        public bool IsHiltFound { get; private set; }

        public CommonDLL.Static.Pickaxe StaticPickaxe { get; }

        public bool IsUnlocked {
            get
            {
                if (StaticPickaxe.Type == PickaxeType.Mythical)
                    return IsHiltFound;

                return App.Instance.Player.Dungeon.LastOpenedTier.Number + 1 >= StaticPickaxe.RequiredTierNumber;
            }
        }

        public Pickaxe(CommonDLL.Static.Pickaxe staticPickaxe, bool isCreated = false, bool isHiltFound = false)
        {
            StaticPickaxe = staticPickaxe;
            IsHiltFound = isHiltFound;
            IsCreated = isCreated;

            Subscribe();
        }

        public Pickaxe(CommonDLL.Static.Pickaxe staticPickaxe, CommonDLL.Dto.Pickaxe dtoPickaxe)
        {
            StaticPickaxe = staticPickaxe;
            IsHiltFound = dtoPickaxe.IsHiltFound;
            IsCreated = dtoPickaxe.IsCreated;

            Subscribe();
        }

        public bool CanCreateDonate()
        {
            if (!IsUnlocked || IsCreated)
            {
                return false;
            }

            if (StaticPickaxe.Type == PickaxeType.Donate)
            {
                var cost = new Dto.Currency(CurrencyType.Crystals, StaticPickaxe.Cost);
                if (!App.Instance.Player.Wallet.Has(cost))
                {
                    return false;
                }
            }

            return true;
        }

        public void Create(Action<bool> onComplete = null, bool force = false)
        {
            if (IsCreated)
            {
                onComplete?.Invoke(false);
                return;
            }

            if (force)
            {
                IsCreated = true;
                IsHiltFound = true;
                onComplete?.Invoke(true);
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeCreate);

                EventManager.Instance.Publish(new PickaxeCreateEvent(this));
                App.Instance.Services.AnalyticsService.CreatePickaxe(StaticPickaxe.Id, StaticPickaxe.Type);
                IsCreated = true;
                return;
            }

            if (StaticPickaxe.Type == PickaxeType.Reward)
            {
                IsCreated = true;
                IsHiltFound = true;
                onComplete?.Invoke(true);
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeCreate);

                App.Instance.Services.AnalyticsService.CreatePickaxe(StaticPickaxe.Id, StaticPickaxe.Type);
                EventManager.Instance.Publish(new PickaxeCreateEvent(this));
                IsCreated = true;
                return;
            }

            if (!IsUnlocked)
            {
                onComplete?.Invoke(false);
                return;
            }

            var ingredients = StaticHelper.GetIngredients(StaticPickaxe);
            if (ingredients.Count > 0 && !App.Instance.Player.Inventory.Has(ingredients))
            {
                onComplete?.Invoke(false);
                return;
            }

            if (StaticPickaxe.Type == PickaxeType.Donate)
            {
                var cost = new Dto.Currency(CurrencyType.Crystals, StaticPickaxe.Cost);
                if (!App.Instance.Player.Wallet.Has(cost))
                {
                    onComplete?.Invoke(false);

                    WindowManager.Instance.Show<WindowShop>()
                        .OpenCrystals();

                    return;
                }

                var staticData = App.Instance.StaticData;

                var pickaxe = staticData.Pickaxes.Find(x => x.Id == StaticPickaxe.Id);

                if (pickaxe != null && pickaxe.Type == PickaxeType.Donate)
                {
                    if (App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, pickaxe.Cost))
                    {
                        App.Instance.Player.Inventory.Remove(ingredients, SpendType.Using);

                        IsCreated = true;
                        App.Instance.Services.AnalyticsService.CreatePickaxe(StaticPickaxe.Id, StaticPickaxe.Type);
                        EventManager.Instance.Publish(new PickaxeCreateEvent(this));

                        onComplete?.Invoke(true);
                    }
                    else
                    {
                        Debug.LogError("Not enough crystals");
                        return;
                    }
                }
                else
                {
                    Debug.LogError("Pickaxe not exist");
                    return;
                }


                return;
            }

            if (StaticPickaxe.Type == PickaxeType.Ad)
            {
                int currentVal;
                App.Instance.Player.Blacksmith.AdPickaxes.TryGetValue(StaticPickaxe.Id, out currentVal);

                if (currentVal < StaticPickaxe.Cost)
                {
                    App.Instance.Player.Blacksmith.SetAdPickaxe(StaticPickaxe.Id);
                    App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.UnlockPickaxe);
                    onComplete?.Invoke(false);
                    return;
                }

                IsCreated = true;
                IsHiltFound = true;
                onComplete?.Invoke(true);
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeCreate);
                App.Instance.Services.AnalyticsService.CreatePickaxe(StaticPickaxe.Id, StaticPickaxe.Type);
                EventManager.Instance.Publish(new PickaxeCreateEvent(this));
                return;
            }

            var goldCost = new Dto.Currency(CurrencyType.Gold, StaticPickaxe.Cost);
            if (!App.Instance.Player.Wallet.Has(goldCost))
            {
                onComplete?.Invoke(false);
                return;
            }

            App.Instance.Player.Inventory.Remove(ingredients, SpendType.Using);
            App.Instance.Player.Wallet.Remove(goldCost);

            IsCreated = true;
            onComplete?.Invoke(true);
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeCreate);
            App.Instance.Services.AnalyticsService.CreatePickaxe(StaticPickaxe.Id, StaticPickaxe.Type);
            EventManager.Instance.Publish(new PickaxeCreateEvent(this));
        }


        private void Subscribe()
        {
            if (IsHiltFound || IsCreated)
                return;

            EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
        }

        private void Unsubscribe()
        {
            EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
        }

        private void OnItemAdd(InventoryItemAddEvent eventData)
        {
            if (IsHiltFound || eventData.Item.Id != StaticPickaxe.Hilt)
                return;

            IsHiltFound = true;
            
            var hiltFindEvent = new PickaxeHiltFindEvent(this);
            EventManager.Instance.Publish(hiltFindEvent);

            Unsubscribe();
        }
    }
}