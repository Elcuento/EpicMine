

using CommonDLL.Static;

namespace BlackTemple.EpicMine.Core
{
    public class Skills
    {
        public SkillLevel Damage { get; private set; }

        public SkillLevel Fortune { get; private set; }

        public SkillLevel Crit { get; private set; }

        public Skills(CommonDLL.Dto.Skills data)
        {
            Damage = new SkillLevel(SkillType.Damage, data?.Damage?.Number ?? 0);
            Fortune = new SkillLevel(SkillType.Fortune, data?.Fortune?.Number ?? 0);
            Crit = new SkillLevel(SkillType.Crit, data?.Critical?.Number ?? 0);
        }

        public void Drop()
        {
            Damage = null;
            Fortune = null;
            Crit = null;
        }
    }
}