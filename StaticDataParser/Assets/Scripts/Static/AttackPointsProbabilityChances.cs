namespace BlackTemple.EpicMine.Static
{
    public class AttackPointsProbabilityChances
    {
        public AttackPointTypeProbability ProbabilityType { get; }

        public int DefaultChance { get; }

        public int EnergyChance { get; }

        public int HealthChance { get; }

        public AttackPointsProbabilityChances(AttackPointTypeProbability probabilityType, int defaultChance, int energyChance, int healthChance)
        {
            ProbabilityType = probabilityType;
            DefaultChance = defaultChance;
            EnergyChance = energyChance;
            HealthChance = healthChance;
        }
    }
}