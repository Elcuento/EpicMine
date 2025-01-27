using System;
using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateAdPickaxe : Response<RequestDataUpdateAdPickaxe>
    {

        public ResponseUpdateAdPickaxe(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Blacksmith?.AdPickaxes == null)
                {
                    Peer.Player.Data.Blacksmith = new Blacksmith()
                    {
                        Pickaxes = new List<Pickaxe>(),
                        AdPickaxes = new Dictionary<string, int>()
                    };
                }

                foreach (var item in Value.Items)
                {
                    var itemData = Peer.Player.Data.Blacksmith.AdPickaxes.ContainsKey(item.Key);

                    if (itemData)
                    {
                        Peer.Player.Data.Blacksmith.AdPickaxes[item.Key] = item.Value;
                    }
                    else
                    {
                        Peer.Player.Data.Blacksmith.AdPickaxes.Add(item.Key, item.Value);
                    }
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}