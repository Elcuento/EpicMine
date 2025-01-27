using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Dto;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;

public class ShopController
{
    private const string FileName = "shopData";
    private const string FileNamePurchase = "purchaseData";

    private readonly IStorageService _storageService = new JsonDiskStorageService();

    public List<ShopTrigger> ShopTriggers;

    public List<Purchase> PurchasesReceipts;


    public ShopController()
    {
        EventManager.Instance.Subscribe<ShopSpecialOfferEvent>(OnGetShopOffer);
        EventManager.Instance.Subscribe<ShopTriggerCompleteEvent>(OnGetTriggerCompleted);
        EventManager.Instance.Subscribe<InitializedIapEvent>(OnIapInitialized);

        PurchasesReceipts = new List<Purchase>();
        ShopTriggers = new List<ShopTrigger>();

        if (_storageService.IsDataExists(FileName))
        {
            var data = _storageService.Load<List<ShopTrigger>>(FileName);

            if (data != null && data.Count > 0)
            {
                ShopTriggers = data;
            }
        }

        if (_storageService.IsDataExists(FileNamePurchase))
        {
            var data = _storageService.Load<List<Purchase>>(FileNamePurchase);

            if (data != null && data.Count > 0)
            {
                PurchasesReceipts = data;
            }
        }

        FillTriggers();
    }

    private void OnIapInitialized(InitializedIapEvent eventData)
    {
        CheckPurchases();
    }

    public void CheckPurchases()
    {
     /*   App.Instance.Services.LogService.Log("Check purchase to validate " + PurchasesReceipts.Count);

        if (PurchasesReceipts.Count > 0)
        {
            AmtNetworkController.Instance.SendNetworkMessage<ResponseDataShopGetPurchaseList>(
                CommandType.ShopGetPurchaseList,
      (listRequest =>
            {
                App.Instance.Services.LogService.Log("Get Server Response Completed Purchases " +
                                                     listRequest.PurchaseList.Count);

                if (listRequest.PurchaseList.Count > 0)
                {
                    PlayerPrefsHelper.Save(PlayerPrefsType.AlreadyPurchased, true);
                    App.Instance.Services.AdvertisementService.DisableForceAds(true);
                }

                foreach (var purchase in listRequest.PurchaseList)
                {
                    //  App.Instance.Services.LogService.Log(purchase.ToJson());
                    var existPurchase = PurchasesReceipts.FindIndex(x => x.receipt == purchase.Receipt);
                    // App.Instance.Services.LogService.Log("Index " + existPurchase);

                    if (existPurchase != -1)
                    {
                        PurchasesReceipts.Remove(PurchasesReceipts[existPurchase]);
                        App.Instance.Services.LogService.Log("Already completed , remove ");
                        Save();
                    }
                }

                for (var i = 0; i < PurchasesReceipts.Count; i++)
                {
                    if (string.IsNullOrEmpty(PurchasesReceipts[i].receipt))
                    {
                        PurchasesReceipts.Remove(PurchasesReceipts[i]);
                        i--;
                    }
                }

                App.Instance.Services.LogService.Log("Rest purchase to validate " + PurchasesReceipts.Count);

                var purchaseList = new List<Purchase>(PurchasesReceipts);

                for (var i = 0; i < purchaseList.Count; i++)
                {
                    var purchasesReceipt = purchaseList[i];

                    App.Instance.Services.LogService.Log("Check " + ":" + purchasesReceipt.shopId + ":" +
                                                         purchasesReceipt.receipt);

                    var result = InAppPurchaseManager.Instance.ValidateReceipt(purchasesReceipt.receipt);

                    App.Instance.Services.LogService.Log("Check purchase result " + result);

                    if (result)
                    {
                        var product = ShopHelper.GetProductByPackId(purchasesReceipt.shopId);
                        var shopPack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == purchasesReceipt.shopId);

                        App.Instance.Services.LogService.Log("Get product " + product);
                        App.Instance.Services.LogService.Log("Get pack " + shopPack);

                        EventManager.Instance.Publish(new IapPurchaseCompleteEvent(product, shopPack,
                            receipt: purchasesReceipt.receipt));
                        App.Instance.Services.LogService.Log("Purchase restore ok");
                    }
                }
            }));
        }
        else
        {
            var result = PlayerPrefsHelper.LoadDefault(PlayerPrefsType.AlreadyPurchased, false);

            App.Instance.Services.AdvertisementService.DisableForceAds(result);

            if (!result)
            {
                AmtNetworkController.Instance.SendNetworkMessage<ResponseDataShopGetPurchaseList>(
                    CommandType.ShopGetPurchaseList,
                    (listRequest =>
                    {
                        if (listRequest.PurchaseList.Count > 0)
                        {
                            PlayerPrefsHelper.Save(PlayerPrefsType.AlreadyPurchased, true);
                            App.Instance.Services.AdvertisementService.DisableForceAds(true);
                        }
                    }));
            }


        }*/
    }

    public void AddPurchaseReceipt(string shopId, string receipt)
    {
        if (string.IsNullOrEmpty(receipt))
            return;

        var exist = PurchasesReceipts.FindIndex(x => x.receipt == receipt);

        if (exist != -1)
        {
            PurchasesReceipts.Remove(PurchasesReceipts[exist]);

            PurchasesReceipts.Add(
                new Purchase {
                    receipt = receipt,
                    shopId = shopId,
                    date = TimeManager.Instance.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture),
                    status = ShopPurchaseStatus.InProgress
                });

            App.Instance.Services.LogService.Log("Receipt already exist, change");
            return;
        }

        PurchasesReceipts.Add(
            new Purchase
            {
                receipt = receipt,
                shopId = shopId,
                date = TimeManager.Instance.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture)
            });

        Save();
    }

    public void RemovePurchaseReceipt(string receipt)
    {
        var exist = PurchasesReceipts.FindIndex(x => x.receipt == receipt);

        if (exist == -1)
            return;

        PurchasesReceipts.Remove(PurchasesReceipts[exist]);
        Save();
    }

    public void OnGetTriggerCompleted(ShopTriggerCompleteEvent eventData)
    {
        var shopPack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == eventData.ShopTrigger.ShopPackId);

        if (shopPack != null)
        {
            switch (shopPack.Type)
            {
                case ShopPackType.Reward:
                    break;
                case ShopPackType.SpecialOffer:
                    App.Instance.Player.Shop.AddShopOffer(shopPack.Id);
                    break;
                default:
                    break;
            }
        }
    }

    public void OnGetShopOffer(ShopSpecialOfferEvent eventData)
    {
        var offerList = new List<CommonDLL.Dto.ShopOffer>();

        if (App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.ShopOfferNotifications))
        {
            offerList = App.Instance.Services.RuntimeStorage.Load<List<CommonDLL.Dto.ShopOffer>>(RuntimeStorageKeys.ShopOfferNotifications);
        }

        if (InAppPurchaseManager.Instance.IsInitialized && SceneManager.Instance.CurrentScene == ScenesNames.Mine)
        {
            if(eventData.Offer.Id == "shop_pack_boss_unsuccesses_inrow_10" 
               || (eventData.Offer.Id == "shop_pack_died_on_monster" &&  !App.Instance.Services.AdvertisementService.IsForceAdsAvailable()))
        
            WindowManager.Instance.Show<WindowShopOffer>(withPause:true)
                .Initialize(new List<CommonDLL.Dto.ShopOffer>
            {
                eventData.Offer
            });

            return;
        }

        offerList.Add(eventData.Offer);

        App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.ShopOfferNotifications, offerList);
    }
    public void Save()
    {
        _storageService.Save(FileName, ShopTriggers);
        _storageService.Save(FileNamePurchase, PurchasesReceipts);
    }

    public ShopTrigger CreateTrigger(string id)
    {
        ShopTrigger trigger = null;

        if (id == "pvp_get_league_3")
            trigger = new ShopTriggerReachPvpLeague(3, "pvp_get_league_3");

        if (id == "pvp_get_league_6")
            trigger = new ShopTriggerReachPvpLeague(6, "pvp_get_league_6");

        if (id == "pvp_get_league_9")
            trigger = new ShopTriggerReachPvpLeague(9, "pvp_get_league_9");

        if (id == "pvp_get_league_12")
            trigger = new ShopTriggerReachPvpLeague(11, "pvp_get_league_11");

        if (id == "pvp_loose_in_row_5")
            trigger = new ShopTriggerPvpLooseInRow(5, "pvp_loose_in_row_5");

        if (id == "shop_pack_workshop_slots_full_4")
            trigger = new ShopTriggerBuyWorkshopSlot(4, "shop_pack_workshop_slots_full_4");

        if (id == "shop_pack_get_royal_chest")
            trigger = new ShopTriggerGetRoyalChest(ChestType.Royal, "shop_pack_get_royal_chest");

        if (id == "shop_pack_takecraft_inrow_100")
            trigger = new ShopTriggerTakeResourcesInRow(100, "shop_pack_takecraft_inrow_100");

        if (id == "shop_pack_boss_unsuccesses_inrow_10")
            trigger = new ShopTriggerDiedOnBoss(3, "shop_pack_boss_unsuccesses_inrow_10");

        if (id == "shop_pack_died_on_monster")
            trigger = new ShopTriggerDiedOnMonster(1, id);

        if (id == "shop_pack_buy_gold_reward")
            trigger = new ShopTriggerBuyGold(ShopLocalConfig.BuyGoldTriggerRequireGold, "shop_pack_buy_gold_reward");

        if (id == "boss_killed_1")
            trigger = new ShopTriggerBossKill(1, "boss_killed_1");

        if (id == "boss_killed_13")
            trigger = new ShopTriggerBossKill(14, "boss_killed_13");

        if (id == "boss_killed_25")
            trigger = new ShopTriggerBossKill(26, "boss_killed_25");

        if (id == "boss_killed_49")
            trigger = new ShopTriggerBossKill(50, "boss_killed_49");


        return trigger;
    }

    public void FillTriggers()
    {
        var allOffers = App.Instance.StaticData.ShopPacks.Where(x => x.Type == ShopPackType.SpecialOffer || x.Type == ShopPackType.Reward).ToList();
        var allPlayerOffers = App.Instance.Player.Shop.ShopOffer;


        // clear if it not exist any more
        for (var i = 0; i < ShopTriggers.Count; i++)
        {
            if (allOffers.Find(x => x.Id == ShopTriggers[i].ShopPackId) == null)
            {
                ShopTriggers.Remove(ShopTriggers[i]);
                i--;
            }
        }

        // check if its already completed
        foreach (var playerOffer in allPlayerOffers)
        {
            var offer = allOffers.Find(x => x.Id == playerOffer.Id);

            if (offer != null)
            {
                if (playerOffer.IsCompleted || !playerOffer.IsActive)
                {
                    allOffers.Remove(offer);
                }
            }
        }

        var newOfferList = new List<ShopPack>();

        foreach (var offer in allOffers)
        {
            if (ShopTriggers.Find(x => x.ShopPackId == offer.Id) != null)
                continue;

            newOfferList.Add(offer);
        }

        foreach (var shopPack in newOfferList)
        {
            var offer = CreateTrigger(shopPack.Id);

            if (offer != null)
                ShopTriggers.Add(offer);
        }

        CheckAll();
        StartAll();
    }

    public void StartAll()
    {
        foreach (var shopOfferTrigger in ShopTriggers)
        {
            shopOfferTrigger.SetStart();
        }
    }

    public void CheckAll()
    {
        foreach (var shopOfferTrigger in ShopTriggers)
        {
            shopOfferTrigger.SetCheck();
        }
    }
}
