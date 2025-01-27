using System;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseLinkWithFaceBook : Response<RequestDataLogin>
    {

        public ResponseLinkWithFaceBook(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            var player = Peer.GetPlayerByFaceBookId(Value.GoogleId);

            if (player == null && string.IsNullOrEmpty(Peer.Player.FaceBookId))
            {
                Peer.Player.SetFaceBookId(Value.FaceBookId);
                Peer.SavePlayer();

                return true;
            }

            return false;
        }
    }
}