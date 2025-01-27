using System;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetLeaderBoardPvp : Response<SendData>
    {

        public ResponseGetLeaderBoardPvp(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Console.WriteLine(Peer.GetPvpBoard().Count);
               ResponseData = new ResponseDataGetLeaderPvpBoard(Peer.GetPvpBoard());
            }

            return true;
        }
    }
}