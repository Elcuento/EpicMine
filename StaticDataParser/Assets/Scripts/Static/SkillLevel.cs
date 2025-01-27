namespace BlackTemple.EpicMine.Static
{
    public class SkillLevel
    {
        public CurrencyType? CostCurrencyType { get; }

        public int CostCurrencyAmount { get; }

        public string CostItemId { get; }

        public int CostItemAmount { get; }

        public float Value { get; }

        public SkillLevel(CurrencyType? costCurrencyType, int costCurrencyAmount, string costItemId, int costItemAmount, float value)
        {
            CostCurrencyType = costCurrencyType;
            CostCurrencyAmount = costCurrencyAmount;
            CostItemId = costItemId.ToLower();
            CostItemAmount = costItemAmount;
            Value = value;
        }
    }
}