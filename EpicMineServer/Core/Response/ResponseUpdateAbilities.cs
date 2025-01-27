using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateAbilities : Response<RequestDataUpdateAbilities>
    {

        public ResponseUpdateAbilities(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Abilities == null)
                {
                    Peer.Player.Data.Abilities = new Abilities();
                }

                foreach (var valueSkill in Value.Skills)
                {
                    switch (valueSkill.Key)
                    {
                        case AbilityType.Acid:
                            Peer.Player.Data.Abilities.Acid = new CommonDLL.Dto.AbilityLevel(valueSkill.Key, valueSkill.Value);
                            break;
                        case AbilityType.Freezing:
                            Peer.Player.Data.Abilities.Freezing = new CommonDLL.Dto.AbilityLevel(valueSkill.Key, valueSkill.Value);
                            break;
                        case AbilityType.ExplosiveStrike:
                            Peer.Player.Data.Abilities.ExplosiveStrike = new CommonDLL.Dto.AbilityLevel(valueSkill.Key, valueSkill.Value);
                            break;
                    }
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}