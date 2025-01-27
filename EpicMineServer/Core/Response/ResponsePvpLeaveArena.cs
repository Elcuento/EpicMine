using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpLeaveArena : Response<SendData>
    {

        public ResponsePvpLeaveArena(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.LeavePvpArena();

                return true;

            }
        }
    }
}