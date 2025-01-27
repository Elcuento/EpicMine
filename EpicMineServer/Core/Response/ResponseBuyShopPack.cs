using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using Buff = CommonDLL.Static.Buff;


namespace AMTServer.Core.Response
{
    public class ResponseBuyShopPack : Response<RequestDataBuyShopPack>
    {

        public ResponseBuyShopPack(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            var isRealPurchase = false;

            var purchases = Peer.GetPurchases();

            if (!string.IsNullOrEmpty(Value.Receipt))
            {
                isRealPurchase = true;

                if (purchases != null)
                {
                    foreach (var purchaseData in purchases.Purchases)
                    {
                        var receipt = purchaseData.Receipt;
                        if (receipt == Value.Receipt)
                        {
                            LogError("Already exist receipt");
                            return false;
                        }
                    }
                }
            }
            

            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var shopPack = staticData.ShopPacks.Find(x => x.Id == Value.Id);
                if (shopPack == null)
                {
                    LogError("not exist shop pack");
                    return false;
                }

                if (shopPack.CrystalCost > 0)
                {
                    if (!Peer.IsCurrencyExist(CurrencyType.Crystals, shopPack.CrystalCost))
                    {
                        Log("not enough money");
                        Peer.SendUpdateWalletCrystals();
                        return false;
                    }

                    if (!Peer.SubsTractCurrency(CurrencyType.Crystals, shopPack.CrystalCost))
                    {
                        Log("not enough money");
                        Peer.SendUpdateWalletCrystals();
                        return false;
                    }
                }

                if (shopPack.Currency.ContainsKey(CurrencyType.Crystals))
                {
                    Peer.AddCurrency(CurrencyType.Crystals, shopPack.Currency[CurrencyType.Crystals]);
                }

                var buffs = new List<Buff>();

                foreach (var shopPackBuff in shopPack.Buffs)
                {
                    var buff = staticData.Buffs.Find(x => x.Id == shopPackBuff);
                    if(buff != null)
                        buffs.Add(buff);
                }

                Peer.AddBuffs(buffs);

                if (shopPack.Type == ShopPackType.SpecialOffer)
                {
                    if (Peer.Player.Data.Shop.ShopOffer == null)
                        Peer.Player.Data.Shop.ShopOffer = new List<ShopOffer>();

                    var offer = Peer.Player.Data.Shop.ShopOffer.Find(x => x.Id == shopPack.Id);

                    if (offer != null)
                        offer.IsCompleted = true;
                }

                if (isRealPurchase)
                {
                    Peer.AddPurchase(shopPack.Id, Value.Receipt,ShopPurchaseStatus.Completed);
                }
               
                Peer.SavePlayer();
            }

            return true;
        }
    }
}
