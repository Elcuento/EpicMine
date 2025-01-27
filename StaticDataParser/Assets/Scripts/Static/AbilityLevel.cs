namespace BlackTemple.EpicMine.Static
{
    public class AbilityLevel
    {
        public CurrencyType? CostCurrencyType { get; }

        public int CostCurrencyAmount { get; }

        public string CostItemId { get; }

        public int CostItemAmount { get; }

        public int Duration { get; }

        public float Damage { get; }

        public float AdditionalParameter { get; }

        public float Cooldown { get; }

        public int EnergyCost { get; }

        public AbilityLevel(CurrencyType? costCurrencyType, int costCurrencyAmount, string costItemId, int costItemAmount, int duration, float damage, float additionalParameter, float cooldown, int energyCost)
        {
            CostCurrencyType = costCurrencyType;
            CostCurrencyAmount = costCurrencyAmount;
            CostItemId = costItemId.ToLower();
            CostItemAmount = costItemAmount;
            Duration = duration;
            Damage = damage;
            AdditionalParameter = additionalParameter;
            Cooldown = cooldown;
            EnergyCost = energyCost;
        }
    }
}