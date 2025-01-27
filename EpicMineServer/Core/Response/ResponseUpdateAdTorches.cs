using System;
using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateAdTorches : Response<RequestDataUpdateAdTorches>
    {

        public ResponseUpdateAdTorches(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.TorchesMerchant?.Torches == null)
                {
                    Peer.Player.Data.TorchesMerchant = new TorchesMerchant()
                    {
                        Torches = new List<Torch>(),
                        AdTorches = new Dictionary<string, int>()
                    };
                }

                foreach (var item in Value.Items)
                {
                    var itemData = Peer.Player.Data.TorchesMerchant.AdTorches.ContainsKey(item.Key);

                    if (itemData)
                    {
                        Peer.Player.Data.TorchesMerchant.AdTorches[item.Key] = item.Value;
                    }
                    else
                    {
                        Peer.Player.Data.TorchesMerchant.AdTorches.Add(item.Key, item.Value);
                    }
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}