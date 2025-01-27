using System;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpJoinCreate : Response<RequestDataPvpJoinCreate>
    {

        public ResponsePvpJoinCreate(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {

                Peer.LeavePvpArena();

                var data = Peer.JoinCreatePvpArena(Value.Arena);

                ResponseData = new ResponseDataPvpJoinCreateArena(data);
                return true;

            }
        }
    }
}