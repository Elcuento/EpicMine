using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateSelectedPickaxe : Response<RequestDataUpdateSelectedPickaxe>
    {

        public ResponseUpdateSelectedPickaxe(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Blacksmith == null)
                {
                    Peer.Player.Data.Blacksmith = new Blacksmith()
                    {
                        Pickaxes = new List<Pickaxe>(),
                        AdPickaxes = new Dictionary<string, int>()
                    };
                }

                Peer.Player.Data.Blacksmith.SelectedPickaxe = Value.Id;
                Peer.SavePlayer();
            }

            return true;
        }
    }
}