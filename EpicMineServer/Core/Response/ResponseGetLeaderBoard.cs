using System;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetLeaderBoard : Response<SendData>
    {

        public ResponseGetLeaderBoard(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
               ResponseData = new ResponseDataGetLeaderBoard(Peer.GetLeaderBoard());
            }

            return true;
        }
    }
}