using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpInviteCancel : Response<RequestDataPvpInviteCancel>
    {

        public ResponsePvpInviteCancel(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.SendInviteCancel(Value.Id);

                return true;

            }
        }
    }
}