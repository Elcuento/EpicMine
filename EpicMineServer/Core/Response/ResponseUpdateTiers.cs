using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateTiers : Response<RequestDataUpdateTiers>
    {

        public ResponseUpdateTiers(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
           lock (Peer.Player)
            {
                if (Peer.Player.Data.Dungeon == null)
                {
                    Peer.Player.Data.Dungeon = new Dungeon
                    {
                        Tiers = new List<Tier>()
                    };
                }
                
                foreach (var item in Value.Items)
                {
                    var itemData = Peer.Player.Data.Dungeon.Tiers.Find(x => x.Number == item.Number);

                    Peer.Player.Data.Dungeon.Tiers.Remove(itemData);

                    Peer.Player.Data.Dungeon.Tiers.Add(item);

                }
                Peer.CalculateMineRating();
                Peer.SavePlayer();
            }

           return true;
        }
    }
}