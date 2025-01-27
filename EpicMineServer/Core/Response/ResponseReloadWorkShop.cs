using AMTServerDLL.Dto;


namespace AMTServer.Core.Response
{
    public class ResponseReloadWorkShop : Response<SendData>
    {

        public ResponseReloadWorkShop(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }


        protected override bool OnProcess()
        {
            ResponseData = new ResponseDataReloadWorkShop(Peer.Player.Data.Workshop);
            return true;
        }
    }
}