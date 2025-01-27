

using CommonDLL.Static;

namespace BlackTemple.EpicMine.Core
{
    public class Abilities
    {
        public AbilityLevel ExplosiveStrike { get; private set; }

        public AbilityLevel Freezing { get; private set; }

        public AbilityLevel Acid { get; private set; }

        public Abilities(CommonDLL.Dto.Abilities data)
        {
            ExplosiveStrike = new AbilityLevel(AbilityType.ExplosiveStrike, data?.ExplosiveStrike?.Number ?? 0);
            Freezing = new AbilityLevel(AbilityType.Freezing, data?.Freezing?.Number ?? 0);
            Acid = new AbilityLevel(AbilityType.Acid, data?.Acid?.Number ?? 0);
        }

        public void Drop()
        {
            ExplosiveStrike = null;
            Freezing = null;
            Acid = null;
        }
    }
}