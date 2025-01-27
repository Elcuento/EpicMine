using System;
using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdatePickaxe : Response<RequestDataUpdatePickaxe>
    {

        public ResponseUpdatePickaxe(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Blacksmith?.Pickaxes == null)
                {
                    Peer.Player.Data.Blacksmith = new Blacksmith()
                    {
                        Pickaxes = new List<Pickaxe>(),
                        AdPickaxes = new Dictionary<string, int>()
                    };
                }

                foreach (var item in Value.Items)
                {
                    var itemData = Peer.Player.Data.Blacksmith.Pickaxes.Find(x => x.Id == item.Id);
                    if (itemData != null)
                    {
                        Peer.Player.Data.Blacksmith.Pickaxes.Remove(itemData);
                    }
                    
                        Peer.Player.Data.Blacksmith.Pickaxes.Add(item);
                    
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}