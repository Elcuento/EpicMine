using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetProfile : Response<SendData>
    {

        public ResponseGetProfile(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            ResponseData = new ResponseDataGetProfile(Peer.Player.Data, Peer.Player.FireBaseId);
            return true;
        }
    }
}