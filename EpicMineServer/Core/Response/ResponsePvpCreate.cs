using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpCreate : Response<RequestDataPvpCreate>
    {

        public ResponsePvpCreate(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.LeavePvpArena();

                var data = Peer.CreatePvpArena(Value.Arena);

                ResponseData = new ResponseDataPvpCreate(data);
                return true;

            }
        }
    }
}