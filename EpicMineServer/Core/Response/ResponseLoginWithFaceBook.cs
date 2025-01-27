using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseLoginWithFaceBook : Response<RequestDataLogin>
    {

        public ResponseLoginWithFaceBook(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            Peer.GetCreatePlayerByFaceBookId(Value.DeviceId, Value.FaceBookId);
            Peer.Player.Data.Location = Value.Localate;
            Peer.Login();
            Peer.FixProfile();

            Peer.SavePlayer();

            if (Peer.GetAndSetVersionInfo(Value.Platform, Value.AppVersion) == null)
            {
                LogError("Version not exist");
                ResponseData = new SendData(new SendDataError(2));
                return false;
            }

            ResponseData = new ResponseDataLogin(Peer.Player?.Data, Peer.Version, Peer.Player?.GoogleId, Peer.Player?.FaceBookId,
                Peer.Player?.FireBaseId);

            return true;
        }
    }
}