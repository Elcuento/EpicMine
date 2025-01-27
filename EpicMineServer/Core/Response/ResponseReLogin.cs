using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseReLogin : Response<RequestDataReLogin>
    {

        public ResponseReLogin(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            var user = Peer.GetAndSetPlayerWithNull(Value.UserId);

            if (Peer.GetAndSetVersionInfo(Value.Platform, Value.Version) == null)
            {
                LogError("Version not exist");
                ResponseData = new SendData(new SendDataError(2));
                return false;
            }

            if (user != null)
            {
                Peer.Login();
                return true;
            }

            return false;
        }
    }
}