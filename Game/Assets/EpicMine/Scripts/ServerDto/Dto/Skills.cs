using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class Skills
    {
        public SkillLevel Damage;

        public SkillLevel Fortune;

        public SkillLevel Critical;

        public Skills()
        {

        }

        public Skills(BlackTemple.EpicMine.Core.Skills data)
        {
            Damage = data.Damage != null ? new SkillLevel(data.Damage.Type, data.Damage.Number) : null;
            Fortune = data.Fortune != null ? new SkillLevel(data.Fortune.Type, data.Fortune.Number) : null;
            Critical = data.Crit != null ? new SkillLevel(data.Crit.Type, data.Crit.Number) : null;
        }
    }
}