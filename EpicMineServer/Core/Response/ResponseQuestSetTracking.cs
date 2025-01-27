using System;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdatePvpInvite : Response<RequestDataUpdatePvpInvite>
    {

        public ResponseUpdatePvpInvite(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            Peer.Player.Data.Pvp.InviteDisable = Value.State;

            Peer.SavePlayer();

            return true;
        }
    }
}