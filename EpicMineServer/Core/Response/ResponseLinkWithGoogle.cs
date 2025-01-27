using System;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseLinkWithGoogle : Response<RequestDataLogin>
    {

        public ResponseLinkWithGoogle(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            var player = Peer.GetPlayerByGoogleId(Value.GoogleId);

            if (player == null && string.IsNullOrEmpty(Peer.Player.GoogleId))
            {
                Peer.Player.SetGoogleId(Value.GoogleId);
                Peer.SavePlayer();

                return true;
            }

            return false;


        }
    }
}