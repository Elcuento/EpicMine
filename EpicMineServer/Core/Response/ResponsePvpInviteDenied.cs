using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpInviteDenied : Response<RequestDataPvpInviteDenied>
    {

        public ResponsePvpInviteDenied(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.SendInviteDenied(Value.Id);

                ResponseData = new ResponseDataPvpInviteDenied(Value.Id);
                return true;

            }
        }
    }
}