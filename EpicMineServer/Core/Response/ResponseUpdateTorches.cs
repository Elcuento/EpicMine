using System;
using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateTorches : Response<RequestDataUpdateTorches>
    {

        public ResponseUpdateTorches(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.TorchesMerchant == null)
                {
                    Peer.Player.Data.TorchesMerchant = new TorchesMerchant()
                    {
                        Torches = new List<Torch>(),
                        AdTorches = new Dictionary<string, int>()
                    };
                }

                foreach (var item in Value.Items)
                {
                    var itemData = Peer.Player.Data.TorchesMerchant.Torches.Find(x => x.Id == item.Id);
                    if (itemData != null)
                    {
                        Peer.Player.Data.TorchesMerchant.Torches.Remove(itemData);
                    }
                    
                        Peer.Player.Data.TorchesMerchant.Torches.Add(item);
                    
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}