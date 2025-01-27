using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseDeveloperSetArtifacts : Response<RequestDataDeveloperSetArtifacts>
    {

        public ResponseDeveloperSetArtifacts(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.Player.Data.Artifacts = Value.Val;
                Peer.SavePlayer();
            }

            return true;
        }
    }
}