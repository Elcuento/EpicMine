using AMTServerDLL;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpFindUserByName : Response<RequestDataPvpFindUser>
    {

        public ResponsePvpFindUserByName(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            var user = Peer.GetByNickName(Value.UserName);
            var online = Peer.GetClientByNickName(Value.UserName);
            var lastTimeOnline = online != null ? Utils.GetUnixTime() : user?.LastOnlineDate;

            if (user == null)
            {
                return false;
            }
            else
            {
                ResponseData = new ResponseDataPvpFindUserByName(user.Data.Id, user.Data.Nickname, user.Data.Blacksmith?.SelectedPickaxe,
                    user.Data?.Pvp?.Rating ?? 0, lastTimeOnline ?? 0);
            }

            return true;
        }
    }
}