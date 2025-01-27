using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpUpdateMatchInfo : Response<RequestDataPvpUpdateMatchInfo>
    {

        public ResponsePvpUpdateMatchInfo(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
               return Peer.PvpUpdateMatchInfo(Value.Info);
            }
        }
    }
}