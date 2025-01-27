using System;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseLoginWithGoogle : Response<RequestDataLogin>
    {

        public ResponseLoginWithGoogle(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            Peer.GetCreatePlayerByGoogleId(Value.DeviceId, Value.GoogleId);
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