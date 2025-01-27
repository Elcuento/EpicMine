using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpInvite : Response<RequestDataPvpInvite>
    {

        public ResponsePvpInvite(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
              return Peer.SendInvite(Value.UserId);
            }
        }
    }
}