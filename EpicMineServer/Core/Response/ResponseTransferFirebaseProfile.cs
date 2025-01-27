using AMTServerDLL;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseTransferFireBaseProfile : Response<RequestDataTransferProfile>
    {

        public ResponseTransferFireBaseProfile(ClientPeer peer, Package pack) : base(peer,  pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.Player.SetData(Value.Player);

                Value.Player.CreationDate =
                    Peer.Player.Data.CreationDate > 0
                        ? Peer.Player.Data.CreationDate / 1000
                        : Utils.GetUnixTime();

                Peer.Player.SetGoogleId(Value.GoogleId);
                Peer.Player.SetFaceBookId(Value.FaceBookId);
                Peer.Player.SetFireBaseId(Value.FireBaseId);

                Peer.Player.Data.Id = Peer.Player.Id.ToString();

                Peer.SavePlayer();
            }

            return true;
        }
    }
}