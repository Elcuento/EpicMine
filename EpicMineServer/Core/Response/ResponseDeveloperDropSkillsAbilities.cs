using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseDeveloperDropSkillsAbilities : Response<SendData>
    {

        public ResponseDeveloperDropSkillsAbilities(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.Player.Data.Skills = new Skills();
                Peer.Player.Data.Abilities = new Abilities();
                Peer.SavePlayer();
            }

            return true;
        }
    }
}