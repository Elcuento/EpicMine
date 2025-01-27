using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpInviteAccepted : Response<RequestDataPvpInviteAccepted>
    {

        public ResponsePvpInviteAccepted(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {

                if (!Peer.SendInviteAccepted(Value.UserId, Value.MatchId, out var matchInfo, out var userInfo))
                    return false;

                ResponseData = new ResponseDataPvpInviteAccepted(userInfo, matchInfo);
                return true;

            }
        }
    }
}