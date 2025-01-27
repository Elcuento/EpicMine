using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetLeaderBoardNewBie : Response<SendData>
    {

        public ResponseGetLeaderBoardNewBie(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
               ResponseData = new ResponseDataGetLeaderBoard(Peer.GetNewBieLeaderBoard());
            }

            return true;
        }
    }
}