using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using Buff = CommonDLL.Static.Buff;


namespace AMTServer.Core.Response
{
    public class ResponseBuyShopPackSubscription : Response<RequestDataBuyShopPackSubscription>
    {

        public ResponseBuyShopPackSubscription(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            var purchases = Peer.GetPurchases();

            if (!string.IsNullOrEmpty(Value.Receipt))
            {
                if (purchases != null)
                {
                    foreach (var purchaseData in purchases.Purchases)
                    {
                        var receipt = purchaseData.Receipt;
                        if (receipt == Value.Receipt)
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                return false;
            }


            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var shopPack = staticData.ShopPacks.Find(x => x.Id == Value.Id);
                if (shopPack == null)
                {
                    return false;
                }


                if (Peer.Player.Data.Shop == null)
                    Peer.Player.Data.Shop = new Shop();

                if (Peer.Player.Data.Shop.ShopSubscription == null)
                    Peer.Player.Data.Shop.ShopSubscription = new List<ShopSubscription>();


                if (shopPack.CrystalCost > 0)
                {
                    if (!Peer.IsCurrencyExist(CurrencyType.Crystals, shopPack.CrystalCost))
                    {
                        return false;
                    }

                    if (!Peer.SubsTractCurrency(CurrencyType.Crystals, shopPack.CrystalCost))
                        return false;
                }

                if (shopPack.Currency.ContainsKey(CurrencyType.Crystals))
                {
                    Peer.AddCurrency(CurrencyType.Crystals, shopPack.Currency[CurrencyType.Crystals]);
                }

                var buffs = new List<Buff>();

                foreach (var shopPackBuff in shopPack.Buffs)
                {
                    var buff = staticData.Buffs.Find(x => x.Id == shopPackBuff);
                    if (buff != null)
                        buffs.Add(buff);
                }

                Peer.AddBuffUntilTime(buffs, Value.Time);

                var subscription = new ShopSubscription
                {
                    Date = Value.Time,
                    Id = shopPack.Id
                };

                Peer.Player.Data.Shop.ShopSubscription.Remove(
                    Peer.Player.Data.Shop.ShopSubscription.Find(x => x.Id == shopPack.Id));

                Peer.Player.Data.Shop.ShopSubscription.Add(subscription);


                Peer.AddPurchase(shopPack.Id, Value.Receipt, ShopPurchaseStatus.Completed);


                Peer.SavePlayer();
            }

            return true;
        }
    }
}
