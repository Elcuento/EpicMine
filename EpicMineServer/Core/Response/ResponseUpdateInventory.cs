using System;
using System.Collections.Generic;
using System.Diagnostics;
using AMTServerDLL;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateInventory : Response<RequestDataUpdateInventory>
    {

        public ResponseUpdateInventory(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
       
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Inventory?.Items == null)
                {
                    Peer.Player.Data.Inventory = new Inventory
                    {
                        Items = new List<Item>()
                    };
                }

                foreach (var item in Value.Items)
                {
                    var itemData = Peer.Player.Data.Inventory.Items.Find(x => x.Id == item.Id);
                    Peer.Player.Data.Inventory.Items.Remove(itemData);

                    if (item.Amount >  0)
                    {
                        Peer.Player.Data.Inventory.Items.Add(new Item(item.Id, item.Amount));
                    }

                }

                Peer.SavePlayer();
            }

            return true;
        }

    }
}