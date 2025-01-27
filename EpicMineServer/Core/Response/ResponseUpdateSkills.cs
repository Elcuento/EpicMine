using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateSkills : Response<RequestDataUpdateSkills>
    {

        public ResponseUpdateSkills(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Skills == null)
                {
                    Peer.Player.Data.Skills = new Skills();
                }

                foreach (var valueSkill in Value.Skills)
                {
                    switch (valueSkill.Key)
                    {
                        case SkillType.Crit:
                            Peer.Player.Data.Skills.Critical = new CommonDLL.Dto.SkillLevel(valueSkill.Key, valueSkill.Value);
                            break;
                        case SkillType.Damage:
                            Peer.Player.Data.Skills.Damage = new CommonDLL.Dto.SkillLevel(valueSkill.Key, valueSkill.Value);
                            break;
                        case SkillType.Fortune:
                            Peer.Player.Data.Skills.Fortune = new CommonDLL.Dto.SkillLevel(valueSkill.Key, valueSkill.Value);
                            break;
                    }
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}