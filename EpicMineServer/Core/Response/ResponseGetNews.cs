using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetNews : Response<SendData>
    {

        public ResponseGetNews(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            ResponseData = new ResponseDataGetNews(Peer.GetNews());
            return true;
        }
    }
}