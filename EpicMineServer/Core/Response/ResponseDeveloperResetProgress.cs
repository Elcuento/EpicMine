using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseDeveloperResetProgress : Response<SendData>
    {

        public ResponseDeveloperResetProgress(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var player = new Player(Peer.Player.Data.Id, Peer.Player.Data.Nickname);

                Peer.Player.SetData(player);

                Peer.SavePlayer();
            }

            return true;
        }
    }
}