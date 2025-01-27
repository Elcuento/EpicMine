using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseDeveloperSetRating : Response<RequestDataDeveloperSetPvpRating>
    {

        public ResponseDeveloperSetRating(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.Player.Data.Pvp.Rating = Value.Val;

                Peer.SavePlayer();
            }

            return true;
        }
    }
}