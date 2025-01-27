using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateSelectedTorch : Response<RequestDataUpdateSelectedTorch>
    {

        public ResponseUpdateSelectedTorch(ClientPeer peer, Package pack) : base(peer, pack)
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

                Peer.Player.Data.TorchesMerchant.SelectedTorch = Value.Id;
                Peer.SavePlayer();
            }

            return true;
        }
    }
}