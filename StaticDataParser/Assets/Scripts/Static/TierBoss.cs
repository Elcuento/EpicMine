namespace BlackTemple.EpicMine.Static
{
    public class TierBoss
    {
        public float Health { get; }

        public float HealPercent { get; }

        public int HealDuration { get; }

        public int HealTimeout { get; }

        public string DropItemId { get; }

        public int AmountMin { get; }

        public int AmountMax { get; }

        public TierBoss(float health, float healPercent, int healDuration, int healTimeout, string dropItemId, int amountMin, int amountMax)
        {
            Health = health;
            HealPercent = healPercent;
            HealDuration = healDuration;
            HealTimeout = healTimeout;
            DropItemId = dropItemId;
            AmountMin = amountMin;
            AmountMax = amountMax;
        }
    }
}