using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponsePvpSendEmoji : Response<RequestDataPvpSendEmoji>
    {

        public ResponsePvpSendEmoji(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
               Peer.SendEmoji(Value.MatchId, Value.Id);
               Peer.SavePlayer();
            }

            return true;
        }
    }
}