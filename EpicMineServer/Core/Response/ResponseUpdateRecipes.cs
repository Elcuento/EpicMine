using System;
using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateRecipes : Response<RequestDataUpdateRecipes>
    {

        public ResponseUpdateRecipes(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Workshop == null)
                {
                    Peer.Player.Data.Workshop = new Workshop
                    {
                        Recipes = new List<Recipe>(),
                        Slots = new List<WorkshopSlot>(),
                        SlotsShard = new List<WorkshopSlot>()
                    };
                }

                foreach (var item in Value.Items)
                {
                    var itemData = Peer.Player.Data.Workshop.Recipes.Find(x => x.Id == item.Id);
                    Peer.Player.Data.Workshop.Recipes.Remove(itemData);

                    Peer.Player.Data.Workshop.Recipes.Add(item);

                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}