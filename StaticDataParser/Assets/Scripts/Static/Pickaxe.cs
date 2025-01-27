namespace BlackTemple.EpicMine.Static
{
    public class Pickaxe
    {
        public string Id { get; }

        public PickaxeType Type { get; }

        public Rarity Rarity { get; }

        public float Damage { get; }

        public string Ingredient1Id { get; }

        public int Ingredient1Amount { get; }

        public string Ingredient2Id { get; }

        public int Ingredient2Amount { get; }

        public string Ingredient3Id { get; }

        public int Ingredient3Amount { get; }

        public string Hilt { get; }

        public int Cost { get; }

        public int RequiredTierNumber { get; }

        public int RequiredDamageLevel { get; }

        public float? BonusGoldPercent { get; }

        public float? BonusDamagePercent { get; }

        public float? BonusFortunePercent { get; }

        public float? BonusCritPercent { get; }

        public string BonusDropItemId { get; }

        public float? BonusDropItemPercent { get; }

        public string HitEffect { get; }

        public Pickaxe(string id, PickaxeType type, Rarity rarity, float damage, string ingredient1Id,
            int ingredient1Amount, string ingredient2Id, int ingredient2Amount, string ingredient3Id,
            int ingredient3Amount, string hilt, int cost, int requiredTierNumber,
            int requiredDamageLevel, float? bonusGoldPercent, float? bonusDamagePercent,
            float? bonusFortunePercent, float? bonusCritPercent, string bonusDropItemId,
            float? bonusDropItemPercent, string hitEffect)
        {
            Id = id;
            Type = type;
            Rarity = rarity;
            Damage = damage;
            Ingredient1Id = ingredient1Id.ToLower();
            Ingredient1Amount = ingredient1Amount;
            Ingredient2Id = ingredient2Id.ToLower();
            Ingredient2Amount = ingredient2Amount;
            Ingredient3Id = ingredient3Id.ToLower();
            Ingredient3Amount = ingredient3Amount;
            Hilt = hilt.ToLower();
            Cost = cost;
            RequiredTierNumber = requiredTierNumber;
            RequiredDamageLevel = requiredDamageLevel;
            BonusGoldPercent = bonusGoldPercent;
            BonusDamagePercent = bonusDamagePercent;
            BonusFortunePercent = bonusFortunePercent;
            BonusCritPercent = bonusCritPercent;
            BonusDropItemId = bonusDropItemId.ToLower();
            BonusDropItemPercent = bonusDropItemPercent;
            HitEffect = hitEffect;
        }
    }
}