using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
// ReSharper disable IdentifierTypo

namespace BlackTemple.EpicMine.Core
{
    public class Shop
    {
        public List<ShopOffer> ShopOffer { get; }
        public List<ShopSale> ShopSale { get; }
        public List<ShopSubscription> ShopSubscription { get; }
        public List<ShopTimerPurchase> TimePurchase { get;  }

        public Shop(CommonDLL.Dto.Shop shop)
        {
            Subscribe();

            TimePurchase = new List<ShopTimerPurchase>();
            ShopOffer = new List<ShopOffer>();
            ShopSale = new List<ShopSale>();
            ShopSubscription = new List<ShopSubscription>();

            if (shop.ShopOffer != null && shop.ShopOffer.Count > 0)
            {
                foreach (var offerDto in shop.ShopOffer)
                {
                    var baseOffer = App.Instance.StaticData.ShopPacks.Find(x => x.Id == offerDto.Id);
                    var offer = new ShopOffer(baseOffer, offerDto);
                    ShopOffer.Add(offer);
                }
            }

            if (shop.ShopSale != null && shop.ShopSale.Count > 0)
            {
                foreach (var shopSale in shop.ShopSale)
                {
                    ShopSale.Add(new ShopSale(shopSale));
                }
            }

            if (shop.TimePurchase != null && shop.TimePurchase.Count > 0)
            {
                foreach (var purchase in shop.TimePurchase)
                {
                    var basePurchase = App.Instance.StaticData.ShopPacks.Find(x => x.Id == purchase.Id);
                    var timePurchase = new ShopTimerPurchase(basePurchase, purchase);
                    timePurchase.RecalculateCharges();
                    TimePurchase.Add(timePurchase);
                }
            }

            if (shop.ShopSubscription != null && shop.ShopSubscription.Count > 0)
            {
                foreach (var shopSubscription in shop.ShopSubscription)
                {
                    ShopSubscription.Add(new ShopSubscription(shopSubscription));
                }
            }

            EventManager.Instance.Subscribe<InitializedIapEvent>(OnIapInitialized);
        }

      

        private void OnIapInitialized(InitializedIapEvent eventData)
        {
            CheckSubscription();
        }

        private void CheckSubscription()
        {
             if (!InAppPurchaseManager.Instance.IsInitialized)
                return;

            App.Instance.Services.LogService.Log("Check subscriptions " + InAppPurchaseManager.Instance.StoreController.products.all.Length);

            var products = InAppPurchaseManager.Instance.StoreController.products.all.ToList();
            foreach (var product in products)
            {
                if (product.definition.type == ProductType.NonConsumable)
                {
                    var result = InAppPurchaseManager.Instance.CheckSubscription(product);

                    if(result == null)
                        continue;

                    if ( (result.isSubscribed() == Result.True || result.isFreeTrial() == Result.True) && result.isExpired() == Result.False)
                    {
                        var subscription = ShopSubscription.Find(x => x.Id == product.definition.id);

                        var time = result.getExpireDate().ToUniversalTime();
                        var timeExpire = TimeManager.Instance.NowUnixSeconds;

                        if (subscription == null || !subscription.IsActive)
                        {
                            App.Instance.Services.LogService.Log("Start Restoring");

                            var shopPack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == product.definition.id);

                            App.Instance.Services.LogService.Log("Expire date " + time + "\n" + timeExpire);

                            ///

                            var staticData = App.Instance.StaticData;

                            var subscriptionPack = staticData.ShopPacks.Find(x => x.Id == shopPack.Id);
                            if (subscriptionPack == null)
                                return;

                            var subscriptionExist =
                                App.Instance.Player.Shop.ShopSubscription.Find(x => x.Id == shopPack.Id);

                            if (subscriptionExist != null)
                                return;

                            var dateNow = TimeManager.Instance.NowUnixSeconds;
                            if (dateNow > timeExpire)
                                return;

                            var buffs = new List<Buff>();
                            foreach (var subscriptionBuff in subscriptionPack.Buffs)
                            {
                                var buffData = staticData.Buffs.Find(x => x.Id == subscriptionBuff);
                                if (buffData != null)
                                    buffs.Add(new Buff(buffData));
                            }

                            // App.Instance.Player.Effect.AddBuffUntilTime(buffs, timeExpire);//.AddBuffUntilTime(buffs, Value.ExpireDate);

                            App.Instance.Player.Shop.ShopSubscription.Add(new ShopSubscription(shopPack.Id,
                                timeExpire)); // { Date = ex, Id = Value.Id });

                            ShopHelper.ExtractBuffs(shopPack, timeExpire, true);

                            App.Instance.Services.LogService.Log("Subscription restored");
                        }
                        else
                        {
                            App.Instance.Services.LogService.Log("Subscription working");
                        }
                    }
                    else
                    {
                        App.Instance.Services.LogService.Log("Subscription expired");
                        var subscription = ShopSubscription.Find(x => x.Id == product.definition.id);

                        if (subscription != null && subscription.IsActive)
                            subscription.Deactivate();
                    }
                }
            }

        }

        private void OnPurchaseFailed(IapPurchaseErrorEvent eventData)
        {
            Debug.LogError(" Purchase error " + eventData.ProductId);

            WindowManager.Instance.Show<WindowAlert>()
                .Initialize($"Purchase error", 5, false);
        }

        private void OnPurchaseCompleted(IapPurchaseCompleteEvent eventData)
        {
       
            var staticShopPack = eventData.Product != null
                ? ShopHelper.GetPackByProductId(eventData.Product)
                : eventData.Pack;

            if (staticShopPack == null)
            {
                Debug.LogError($"Shop pack not exist {eventData.Pack.Id}");
                return;
            }

            var receipt = eventData.Receipt != ""
                ? eventData.Receipt
                : (eventData.Product != null ? eventData.Product.receipt : "");

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(receipt))
                receipt = Guid.NewGuid().ToString();
#endif

            var id = staticShopPack.Id;

            Debug.Log("buy shop pack " + id);


            if (staticShopPack.Cost > 0)
            {
                App.Instance.Controllers.ShopController.AddPurchaseReceipt(staticShopPack.Id, receipt);
            }

            switch (staticShopPack.Type)
            {
                case ShopPackType.Alchemy:
                case ShopPackType.Dragon:
                case ShopPackType.Miner:
                    BuyShopPackSaleRequest(staticShopPack.Id, receipt, (sale =>
                    {
                        if (staticShopPack.CrystalCost > 0)
                        {
                            var currency = new Currency(CurrencyType.Crystals, staticShopPack.CrystalCost);
                            App.Instance.Player.Wallet.Remove(currency);
                        }

                        ShopHelper.ExtractShopPack(staticShopPack);
                        BroadcastBuyShopPack(staticShopPack);
                    }));

                    break;
                case ShopPackType.SpecialOffer:
                    if (staticShopPack.Cost == 0 && staticShopPack.CrystalCost == 0)
                    {
                        ShopHelper.ExtractShopPack(staticShopPack);
                        BroadcastBuyShopPack(staticShopPack);
                    }
                    else
                    {
                        BuyShopPackRequest(staticShopPack, receipt, () =>
                        {
                            if (staticShopPack.CrystalCost > 0)
                            {
                                var currency = new Currency(CurrencyType.Crystals, staticShopPack.CrystalCost);
                                App.Instance.Player.Wallet.Remove(currency);
                            }

                            var offer = ShopOffer.Find(x => x.Id == id);
                            offer?.SetCompleted();
                            ShopHelper.ExtractShopPack(staticShopPack);
                            BroadcastBuyShopPack(staticShopPack);
                        });
                    }
                   
                    break;
                case ShopPackType.Subscription:

                    var existPack = ShopSubscription.Find(x => x.Id == staticShopPack.Id);

                    var alreadyBuyed = existPack != null && existPack.IsActive;

                    if (alreadyBuyed)
                        return;

                    BuyShopPackSubscriptionRequest(staticShopPack, eventData.Product, receipt, (expireTime) =>
                    {
                        if (existPack != null)
                        {
                            existPack.Update(new Dto.ShopSubscription(existPack.Id, expireTime));
                        }
                        else
                        {
                            ShopSubscription.Add(new ShopSubscription(staticShopPack.Id, expireTime));
                        }

                        ShopHelper.ExtractShopPack(staticShopPack, noBuff: true);
                        ShopHelper.ExtractBuffs(staticShopPack, expireTime);
                        BroadcastBuyShopPack(staticShopPack);
                    });
                    break;

                case ShopPackType.AdReward:
                case ShopPackType.Gold:
                    BuyShopPackTimeRequest(staticShopPack.Id, () =>
                    {
                        if (staticShopPack.CrystalCost > 0)
                        {
                            var currency = new Currency(CurrencyType.Crystals, staticShopPack.CrystalCost);
                            App.Instance.Player.Wallet.Remove(currency);
                        }
                        ShopHelper.ExtractShopPack(staticShopPack);
                        BroadcastBuyShopPack(staticShopPack);
                    });
                    break;
                default:
                    BuyShopPackRequest(staticShopPack, receipt, () =>
                    {
                        if (staticShopPack.CrystalCost > 0)
                        {
                            var currency = new Currency(CurrencyType.Crystals, staticShopPack.CrystalCost);
                            App.Instance.Player.Wallet.Remove(currency);
                        }

                        ShopHelper.ExtractShopPack(staticShopPack);
                        BroadcastBuyShopPack(staticShopPack);
                    });
                    break;
            }

            App.Instance.Services.AnalyticsService.CustomEvent("iap_purchase", new CustomEventParameters
            {
                String = new Dictionary<string, string>
                {
                    {"id" , id }
                }
            });


            if (staticShopPack.Type == ShopPackType.Subscription)
            {
                var result = InAppPurchaseManager.Instance.CheckSubscription(eventData.Product);

                if (result == null || result.isFreeTrial() == Result.True)
                    return;
            }

            if (eventData.Product != null)
            {
                App.Instance.Services.AdvertisementService.DisableForceAds(true);
                PlayerPrefsHelper.Save(PlayerPrefsType.AlreadyPurchased, true);

                App.Instance.Services.AnalyticsService.InAppPurchase(id, "ShopPacks", 1, staticShopPack.Cost, "Real");
            } else { 
                App.Instance.Services.AnalyticsService.InAppPurchase(id, "ShopPacks", 1, staticShopPack.CrystalCost, staticShopPack.CrystalCost > 0 ? "Crystals" : "Free/Ad");
            }
        }

        private void BroadcastBuyShopPack(ShopPack pack)
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Pay);
            EventManager.Instance.Publish(new ShopBuyShopPackEvent(pack));
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<IapPurchaseCompleteEvent>(OnPurchaseCompleted);
            EventManager.Instance.Subscribe<IapPurchaseErrorEvent>(OnPurchaseFailed);
        }



        public bool IsOfferExist(string id)
        {
            return ShopOffer.Find(x => x.Id == id)!=null;
        }


        public void BuyShopPack(Product product)
        {
            if (DeveloperManager.IsDebug)
            {
                EventManager.Instance.Publish(new IapPurchaseCompleteEvent(product));
                return;
            }
#if UNITY_EDITOR
            EventManager.Instance.Publish(new IapPurchaseCompleteEvent(product));
            return;
#endif
            InAppPurchaseManager.Instance.Buy(product.definition.id);
        }

        public void BuyShopPack(ShopPack pack, bool confirm = true)
        {
            if (pack.Cost > 0)
                return;

            if (pack.CrystalCost > 0)
            {
                var cost = new Currency(CurrencyType.Crystals, pack.CrystalCost);

                if (!App.Instance.Player.Wallet.Has(cost))
                    return;

                if (confirm)
                {
                    WindowManager
                        .Instance
                        .Show<WindowCurrencySpendConfirm>()
                        .Initialize(
                            cost,
                            () => { EventManager.Instance.Publish(new IapPurchaseCompleteEvent(pack: pack)); },
                            "window_currency_spend_confirm_description_shop",
                            "window_currency_spend_confirm_ok_shop");
                }else { EventManager.Instance.Publish(new IapPurchaseCompleteEvent(pack: pack)); }
            }

            if(pack.Cost == 0 && pack.CrystalCost == 0)
            {
                EventManager.Instance.Publish(new IapPurchaseCompleteEvent(pack: pack));
            }
        }

        private void BuyShopPackSaleRequest(string id, string receipt, Action<ShopSale> onAdded = null,
            Action onError = null)
        {
            var shopPack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == id);

    
                if (shopPack == null)
                {
                    Debug.LogError("Shot pack not exist");
                    return ;
                }

                
                var saleData = ShopSale.Find(x => x.Type == shopPack.Type);

                var dateNow = TimeManager.Instance.NowUnixSeconds;

                var existSale = new ShopSale(shopPack.Id, shopPack.Type, 0, 1, 1);
                

                if (saleData == null)
                {
                    ShopSale.Add(existSale);

                    foreach (var i in shopPack.Currency)
                    {
                        if (i.Key == CurrencyType.Crystals)
                            App.Instance.Player.Wallet.Add(CurrencyType.Crystals, i.Value, IncomeSourceType.FromBuy);
                    }
                }
                else
                {
                    existSale = saleData;

                    if (existSale.Date < dateNow)
                    {
                        existSale.SetCharge(1);
                        existSale.SetBuyCharge(1);
                    }

                    foreach (var i in shopPack.Currency)
                    {
                        if (i.Key == CurrencyType.Crystals)
                            App.Instance.Player.Wallet.Add(CurrencyType.Crystals, i.Value, IncomeSourceType.FromBuy);
                    }
                }

                if (existSale.Charge < 6)
                {
                    existSale.SetCharge(existSale.Charge + 1);
                    existSale.SetBuyCharge(existSale.Charge - 1);

                }
                else
                {
                    existSale.SetBuyCharge(existSale.Charge);
                }

                existSale.SetDate(dateNow + (10 * 60 * 60));



             /* var sale = ShopSale.Find(x => x.Type == shopPack.Type);
                if (sale == null)
                {
                    sale = new ShopSale(id, shopPack.Type, completeResponse.Date, completeResponse.Charge, completeResponse.BuyCharge);
                    ShopSale.Add(sale);
                }
                sale.Update(new ShopSale(id, shopPack.Type, completeResponse.Date, completeResponse.Charge, completeResponse.BuyCharge));
            */
                onAdded?.Invoke(existSale);
        }

        private void BuyShopPackTimeRequest(string id, Action onAdded = null, Action onError = null)
        {
            var shopPack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == id);

            if (shopPack == null)
            {
                Debug.LogError("Shot pack not exist");
                return;
            }

            var dateNow = TimeManager.Instance.NowUnixSeconds;

            var timePurchaseData =TimePurchase.Find(x => x.Id == id);
          //  var exitTimePurchase = new ShopTimerPurchase(shopPack, 0, shopPack.Charge ?? 0);

            var timeCost = shopPack.Time * 60 * 60 ?? 0;

            if (timePurchaseData == null)
            {
                timePurchaseData = new ShopTimerPurchase(shopPack, dateNow + timeCost, (shopPack.Charge ?? 1) - 1);

                TimePurchase.Add(timePurchaseData);
            }
            else
            {
                if (timePurchaseData.Charge > 0)
                {
                    timePurchaseData.SetCharge(timePurchaseData.Charge - 1);
                    timePurchaseData.SetDate(dateNow + timeCost);
                }
                else
                {
                    if (dateNow > timePurchaseData.Date)
                    {
                        var totalTimeLeft = dateNow - timePurchaseData.Date;
                        var extraCharges = 1;

                        while (extraCharges < shopPack.Charge && totalTimeLeft > timeCost)
                        {
                            extraCharges = extraCharges + 1;
                            totalTimeLeft = totalTimeLeft - timeCost;
                        }

                        timePurchaseData.SetDate(dateNow + timeCost);
                        timePurchaseData.SetCharge(extraCharges - 1);
                    }
                }
            }

            onAdded?.Invoke();
        }

        private void BuyShopPackRequest(ShopPack shopPack, string receipt, Action onBuy = null, Action onFailed = null)
        {

            var cost = new Currency(CurrencyType.Crystals, shopPack.CrystalCost);

            if (!App.Instance.Player.Wallet.Has(cost))
                return;

            var staticData = App.Instance.StaticData;

            if (shopPack == null)
            {
                Debug.LogError("not exist shop pack");
                return;
            }

            if (shopPack.CrystalCost > 0)
            {
                if (!App.Instance.Player.Wallet.Has(CurrencyType.Crystals, shopPack.CrystalCost))
                {
                    Debug.LogError("not enough money");
                    return;
                }

                if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, shopPack.CrystalCost))
                {
                    Debug.LogError("not enough money");
                    return;
                }
            }

            if (shopPack.Currency.ContainsKey(CurrencyType.Crystals))
            {
                App.Instance.Player.Wallet.Add(CurrencyType.Crystals, shopPack.Currency[CurrencyType.Crystals], IncomeSourceType.FromShopBuy);
            }

            var buffs = new List<Buff>();

            foreach (var shopPackBuff in shopPack.Buffs)
            {
                var buff = staticData.Buffs.Find(x => x.Id == shopPackBuff);
                if (buff != null)
                    buffs.Add(new Buff(buff));
            }

            App.Instance.Player.Effect.AddBuffs(buffs);

            if (shopPack.Type == ShopPackType.SpecialOffer)
            {
                var offer = ShopOffer.Find(x => x.Id == shopPack.Id);

                if (offer != null)
                    offer.IsCompleted = true;
            }

            onBuy?.Invoke();
        }

        private void BuyShopPackSubscriptionRequest(ShopPack pack, Product product, string reciept, Action<long> onBuy = null, Action onFailed = null)
        {
            if (!InAppPurchaseManager.Instance.IsInitialized)
                return;

            long expireDate = 0;

#if UNITY_ANDROID || UNITY_IPHONE
            var result = InAppPurchaseManager.Instance.CheckSubscription(product);

            Debug.Log("buy subscription pocess , result is " + result);

            if (result == null)
            {
                Debug.Log("Subscription not exist");
               var date = DateTime.Now.AddHours((double) pack.Time);
              //  Debug.Log("Subscription exist date end " + date);
              expireDate = TimeManager.Instance.NowUnixSeconds;
              //  Debug.Log("Subscription exist date end " + expireDate);
            }
            else
            {
    
                expireDate = new DateTimeOffset(result.getExpireDate().ToUniversalTime()).ToUnixTimeSeconds();
                   Debug.Log("Subscription exist date end " + expireDate);
               
            }
#else
        expireDate = new DateTimeOffset(DateTime.UtcNow.AddDays(30)).ToUnixTimeSeconds();  // TODO CHECK
#endif

            var staticData = App.Instance.StaticData;


            if (pack.CrystalCost > 0)
            {

                if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, pack.CrystalCost))
                {
                    Debug.LogError("Dont have crystals");
                    return;
                }
            }

            if (pack.Currency.ContainsKey(CurrencyType.Crystals))
            {
                App.Instance.Player.Wallet.Add(CurrencyType.Crystals, pack.Currency[CurrencyType.Crystals],IncomeSourceType.FromBuy);
            }

            var buffs = new List<Buff>();

            foreach (var shopPackBuff in pack.Buffs)
            {
                var buff = staticData.Buffs.Find(x => x.Id == shopPackBuff);
                if (buff != null)
                    buffs.Add(new Buff(buff));
            }

            App.Instance.Player.Effect.AddBuffUntilTime(buffs, expireDate);

            var subscription = new ShopSubscription(pack.Id, expireDate);
            
            App.Instance.Player.Shop.ShopSubscription.Remove(
                App.Instance.Player.Shop.ShopSubscription.Find(x => x.Id == pack.Id));

            App.Instance.Player.Shop.ShopSubscription.Add(subscription);

            App.Instance.Player.Save();

        }
        //

        public void AddShopOffer(string id, Action onAdded = null, Action onError = null)
        {
            if (string.IsNullOrEmpty(id))
                return;
            
            if (IsOfferExist(id))
                return;


            var staticData = App.Instance.StaticData;

            var shopPack = staticData.ShopPacks.Find(x => x.Id == id);
            if (shopPack == null)
            {
                Debug.LogError("Shop pack not exist");
                return;
            }


            var offerData = ShopOffer.Find(x => x.Id == id);

            if (offerData != null)
            {
                Debug.LogError("Offer data exist");
                return;
            }

            var date = TimeManager.Instance.NowUnixSeconds + shopPack.Time ?? 0 * 60 * 60;

            ShopOffer.Add(new ShopOffer(shopPack, date));

            App.Instance.Player.Save();

            var offerDto = new CommonDLL.Dto.ShopOffer()
            {
                Date = date,
                Id = id,
                IsCompleted = false
            };
            EventManager.Instance.Publish(new ShopSpecialOfferEvent(offerDto));

            onAdded?.Invoke();

        }

        public bool HasActiveSubscription()
        {
            return ShopSubscription.Find(x => x.IsActive) != null;
        }
    }
}