using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpGetConfirmResult : Response<SendData>
    {

        public ResponsePvpGetConfirmResult(ClientPeer peer, Package pack) : base(peer, pack)
        {
            Peer.PvpConfirmResult();
        }

        protected override bool OnProcess()
        {
            return true;
        }
    }
}