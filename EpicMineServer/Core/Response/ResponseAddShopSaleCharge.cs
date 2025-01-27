using System;
using System.Collections.Generic;
using AMTServer.Common;
using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseAddShopSaleCharge : Response<RequestDataAddShopSaleCharge>
    {

        public ResponseAddShopSaleCharge(ClientPeer peer, Package pack) : base(peer, pack)
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

                if (Peer.Player.Data.Shop.ShopSale == null)
                    Peer.Player.Data.Shop.ShopSale = new List<ShopSale>();

                var saleData = Peer.Player.Data.Shop.ShopSale.Find(x => x.Type == shopPack.Type);

                var dateNow = Utils.GetUnixTime();

                var existSale = new ShopSale
                {
                    Id = shopPack.Id,
                    Charge = 1,
                    Date = 0,
                    Type = shopPack.Type,
                    BuyCharge = 1
                };


                if (saleData == null)
                {
                    Peer.Player.Data.Shop.ShopSale.Add(existSale);

                    foreach (var i in shopPack.Currency)
                    {
                        if (i.Key == CurrencyType.Crystals)
                            Peer.AddCurrency(CurrencyType.Crystals, i.Value);
                    }
                }
                else
                {
                    existSale = saleData;

                    if (existSale.Date < dateNow)
                    {
                        existSale.Charge = 1;
                        existSale.BuyCharge = 1;
                    }

                    foreach (var i in shopPack.Currency)
                    {
                        if (i.Key == CurrencyType.Crystals)
                            Peer.AddCurrency(CurrencyType.Crystals, i.Value);
                    }
                }

                if (existSale.Charge < 6)
                {
                    existSale.Charge = existSale.Charge + 1;
                    existSale.BuyCharge = existSale.Charge - 1;
                }
                else
                {
                    existSale.BuyCharge = existSale.Charge;
                }

                existSale.Date = dateNow + (10 * 60 * 60 );

                Peer.AddPurchase(shopPack.Id, Value.Receipt, ShopPurchaseStatus.Completed);

                ResponseData = new ResponseDataAddShopSaleCharge(existSale.Date, existSale.Charge, existSale.BuyCharge);
                Peer.SavePlayer();
            }

            return true;
        }
    }
}
