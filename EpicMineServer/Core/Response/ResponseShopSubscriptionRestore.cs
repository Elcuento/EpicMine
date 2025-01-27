using System.Collections.Generic;
using AMTServer.Common;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseShopSubscriptionRestore : Response<RequestDataShopSubscriptionRestore>
    {

        public ResponseShopSubscriptionRestore(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var subscription = staticData.ShopPacks.Find(x => x.Id == Value.Id);
                if (subscription == null)
                    return false;

                var subscriptionExist = Peer.Player.Data.Shop.ShopSubscription.Find(x => x.Id == Value.Id);

                if (subscriptionExist != null)
                    return false;

                var dateNow = Utils.GetUnixTime();
                if (dateNow > Value.ExpireDate)
                    return false;

                var buffs = new List<CommonDLL.Static.Buff>();
                foreach (var subscriptionBuff in subscription.Buffs)
                {
                    var buffData = staticData.Buffs.Find(x => x.Id == subscriptionBuff);
                    if(buffData != null)
                    buffs.Add(buffData);
                }

                Peer.AddBuffUntilTime(buffs, Value.ExpireDate);

                Peer.Player.Data.Shop.ShopSubscription.Add(new ShopSubscription{ Date = Value.ExpireDate, Id = Value.Id});
                ResponseData = new ResponseDataShopSubscriptionRestore(Value.ExpireDate);
                return true;
            }
            
        }
    }
}