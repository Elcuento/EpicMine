using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetStaticData : Response<SendData>
    {

        public ResponseGetStaticData(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            ResponseData = new ResponseDataGetStaticData(Peer.GetStaticData());
            return true;
        }
    }
}