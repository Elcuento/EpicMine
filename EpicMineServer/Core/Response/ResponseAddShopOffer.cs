using System;
using System.Collections.Generic;
using AMTServer.Common;
using AMTServerDLL.Dto;
using CommonDLL.Dto;


namespace AMTServer.Core.Response
{
    public class ResponseAddShopOffer : Response<RequestDataAddShopOffer>
    {

        public ResponseAddShopOffer(ClientPeer peer, Package pack) : base(peer, pack)
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
                    LogError("Shop pack not exist");
                    return false;
                }

                if (Peer.Player.Data.Shop == null)
                   Peer.Player.Data.Shop = new Shop();
                

                if(Peer.Player.Data.Shop.ShopOffer == null)
                    Peer.Player.Data.Shop.ShopOffer = new List<ShopOffer>();

                var offerData = Peer.Player.Data.Shop.ShopOffer.Find(x => x.Id == Value.Id);

                if (offerData != null)
                {
                    LogError("Offer data exist");
                    return false;
                }

                var date = Utils.GetUnixTime() + shopPack.Time ?? 0 * 60 * 60;

                Peer.Player.Data.Shop.ShopOffer.Add(
                    new ShopOffer
                {
                    Id = shopPack.Id,
                    IsCompleted = false,
                    Date = date,
                });

                ResponseData = new ResponseDataAddOffer(date);

                Peer.SavePlayer();
            }

            return true;
        }
    }
}