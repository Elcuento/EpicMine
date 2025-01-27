using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetVersionInfo : Response<SendData>
    {

        public ResponseGetVersionInfo(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            ResponseData = new ResponseDataGetVersionInfo(Peer.GetVersionsInfo());
            return true;
        }
    }
}