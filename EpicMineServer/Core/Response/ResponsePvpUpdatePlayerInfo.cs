using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpUpdatePlayerInfo : Response<RequestDataPvpUpdatePlayerInfo>
    {

        public ResponsePvpUpdatePlayerInfo(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                return Peer.PvpUpdateUserInfo(Value.Info);
            }
        }
    }
}