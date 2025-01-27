using System;
using System.Collections.Generic;
using AMTServer.Common;
using AMTServerDLL.Dto;
using CommonDLL.Dto;


namespace AMTServer.Core.Response
{
    public class ResponseAddShopTimePurchase : Response<RequestDataAddShopTimePurchase>
    {

        public ResponseAddShopTimePurchase(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
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

                if (Peer.Player.Data.Shop.TimePurchase == null)
                    Peer.Player.Data.Shop.TimePurchase = new List<ShopTimerPurchase>();

                var dateNow = Utils.GetUnixTime();

                var timePurchaseData = Peer.Player.Data.Shop.TimePurchase.Find(x => x.Id == Value.Id);
                var exitTimePurchase = new ShopTimerPurchase {Id = shopPack.Id, Charge = shopPack.Charge ?? 0, Date = 0};
                var timeCost = shopPack.Time * 60 * 60 ?? 0;

                if (timePurchaseData == null)
                {
                    exitTimePurchase.Date = dateNow + timeCost;
                    exitTimePurchase.Charge = (shopPack.Charge ?? 1) - 1;
         
                    Peer.Player.Data.Shop.TimePurchase.Add(exitTimePurchase);
                }
                else
                {
                    exitTimePurchase = timePurchaseData;

                    if (exitTimePurchase.Charge > 0)
                    {
                        exitTimePurchase.Charge = exitTimePurchase.Charge - 1;
                        exitTimePurchase.Date = dateNow + timeCost;
                    }
                    else
                    {
                        if (dateNow > exitTimePurchase.Date)
                        {
                            var totalTimeLeft = dateNow - exitTimePurchase.Date;
                            var extraCharges = 1;

                            while (extraCharges < shopPack.Charge && totalTimeLeft > timeCost)
                            {
                                extraCharges = extraCharges + 1;
                                totalTimeLeft = totalTimeLeft - timeCost;
                            }

                            exitTimePurchase.Date = dateNow + timeCost;
                            exitTimePurchase.Charge = extraCharges - 1;

                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                ResponseData = new ResponseDataShopBuyTimePurchase(exitTimePurchase.Date,exitTimePurchase.Charge);
                Peer.SavePlayer();
            }

            return true;
        }
    }
}