namespace CommonDLL.Dto
{
    public class Abilities
    {
        public AbilityLevel ExplosiveStrike;

        public AbilityLevel Freezing;

        public AbilityLevel Acid;

        public Abilities()
        {

        }
        public Abilities(BlackTemple.EpicMine.Core.Abilities data )
        {
            ExplosiveStrike = data.ExplosiveStrike != null
                ? new AbilityLevel(data.ExplosiveStrike.Type, data.ExplosiveStrike.Number)
                : null;
            Freezing = data.Freezing != null
                ? new AbilityLevel(data.Freezing.Type, data.Freezing.Number)
                : null; ;
            Acid = data.Acid != null
                ? new AbilityLevel(data.Acid.Type, data.Acid.Number)
                : null; ;
        }
    }
}