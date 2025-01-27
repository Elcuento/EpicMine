using System.Collections.Generic;
using AMTServer.Common;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseShopGetPurchaseList : Response<RequestDataShopSubscriptionRestore>
    {

        public ResponseShopGetPurchaseList(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {

                var purchases = Peer.GetPurchases();
                var purchasesList = new List<Purchase>();

                if (purchases != null)
                {
                    foreach (var p in purchases.Purchases)
                    {
                        purchasesList.Add(p);
                    }
                }

                ResponseData = new ResponseDataShopGetPurchaseList(purchasesList);
                return true;
            }
            
        }
    }
}