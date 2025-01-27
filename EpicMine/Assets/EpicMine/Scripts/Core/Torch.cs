using System;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class Torch
    {
        public bool IsCreated { get; private set; }

        public bool IsUnlocked
        {
            get
            {
                if (App.Instance.Player.Inventory.GetExistAmount(StaticTorch.Id) > 0)
                    return true;

                return PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating) + 1 >= StaticTorch.LeagueId;
            }
        }

        public bool CanCreate()
        {
            var ingredients = StaticHelper.GetIngredients(StaticTorch);
            if (ingredients.Count > 0 && !App.Instance.Player.Inventory.Has(ingredients))
            {
                return false;
            }

            var goldCost = new Dto.Currency(CurrencyType.Gold, StaticTorch.Cost);
            if (!App.Instance.Player.Wallet.Has(goldCost))
            {
                return false;
            }

            return true;
        }

        public CommonDLL.Static.Torch StaticTorch { get; private set; }


        public Torch(CommonDLL.Static.Torch staticTorch, bool isCreated = false)
        {
            StaticTorch = staticTorch;
            IsCreated = isCreated;
        }


        public Torch(CommonDLL.Static.Torch staticPickaxe, CommonDLL.Dto.Torch dtoTorch)
        {
            StaticTorch = staticPickaxe;
            IsCreated = dtoTorch.IsCreated;
        }
        
        public Torch(CommonDLL.Static.Torch staticPickaxe, Torch dtoTorch)
        {
            StaticTorch = staticPickaxe;
            IsCreated = dtoTorch.IsCreated;
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
                onComplete?.Invoke(true);
                EventManager.Instance.Publish(new TorchCreateEvent(this));
                App.Instance.Services.AnalyticsService.CreateTorch(StaticTorch.Id, StaticTorch.Type);
                return;
            }

            if (StaticTorch.Type == TorchType.Reward)
            {
                IsCreated = true;
                onComplete?.Invoke(true);
                EventManager.Instance.Publish(new TorchCreateEvent(this));
                App.Instance.Services.AnalyticsService.CreateTorch(StaticTorch.Id, StaticTorch.Type);
                return;
            }

            if (!IsUnlocked)
            {
                onComplete?.Invoke(false);
                return;
            }

            var ingredients = StaticHelper.GetIngredients(StaticTorch);
            if (ingredients.Count > 0 && !App.Instance.Player.Inventory.Has(ingredients))
            {
                onComplete?.Invoke(false);
                return;
            }

            if (StaticTorch.Type == TorchType.Merchant)
            {
                var goldCost = new Dto.Currency(CurrencyType.Gold, StaticTorch.Cost);
                if (!App.Instance.Player.Wallet.Has(goldCost))
                {
                    onComplete?.Invoke(false);
                    return;
                }
                App.Instance.Player.Inventory.Remove(ingredients, SpendType.Using);
                App.Instance.Player.Wallet.Remove(goldCost);

                IsCreated = true;
                onComplete?.Invoke(true);
                EventManager.Instance.Publish(new TorchCreateEvent(this));
                App.Instance.Services.AnalyticsService.CreateTorch(StaticTorch.Id, StaticTorch.Type);
            } 
            else
            {
                int currentVal;
                App.Instance.Player.TorchesMerchant.AdTorches.TryGetValue(StaticTorch.Id, out currentVal);

                if (currentVal < StaticTorch.Cost)
                {
                    App.Instance.Player.TorchesMerchant.SetAdTorch(StaticTorch.Id);
                    App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.UnlockTorch);
                    onComplete?.Invoke(false);
                    return;
                }

                IsCreated = true;
                onComplete?.Invoke(true);
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeCreate);
                EventManager.Instance.Publish(new TorchCreateEvent(this));
                App.Instance.Services.AnalyticsService.CreateTorch(StaticTorch.Id, StaticTorch.Type);
                return;

            }
        }
    }
}