using System.Globalization;
using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class Torch
    {
        public string Id { get; }

        public int LeagueId { get; }

        public Rarity Rarity { get; }

        public TorchType Type { get; }

        public string Ingredient1Id { get; }

        public int Ingredient1Amount { get; }

        public string Ingredient2Id { get; }

        public int Ingredient2Amount { get; }

        public string Ingredient3Id { get; }

        public int Ingredient3Amount { get; }

        public string Shard { get; }

        public int Cost { get; }

        public int RequiredFortuneLevel { get; }

        public float? ExplosiveStrikeDamage { get; }

        public float? ExplosiveStrikeCooldown { get; }

        public float? FreezingDamage { get; }

        public float? FreezingAdditionalParameter { get; }

        public float? FreezingCooldown { get; }

        public float? AcidDamage { get; }

        public float? AcidCooldown { get; }

        public Torch(string id, int leagueId, Rarity rarity, TorchType type, string ingredient1Id, int ingredient1Amount,
            string ingredient2Id, int ingredient2Amount, string ingredient3Id, int ingredient3Amount, string shard,
            int cost, int requiredFortuneLevel, float? explosiveStrikeDamage, string explosiveStrikeCooldown,
            float? freezingDamage, float? freezingAdditionalParameter, string freezingCooldown, float? acidDamage,
            string acidCooldown)
        {
            Id = id.ToLower();
            LeagueId = leagueId;
            Rarity = rarity;
            Type = type;
            Ingredient1Id = ingredient1Id.ToLower();
            Ingredient1Amount = ingredient1Amount;
            Ingredient2Id = ingredient2Id.ToLower();
            Ingredient2Amount = ingredient2Amount;
            Ingredient3Id = ingredient3Id.ToLower();
            Ingredient3Amount = ingredient3Amount;
            Shard = shard.ToLower();
            Cost = cost;
            RequiredFortuneLevel = requiredFortuneLevel;
            ExplosiveStrikeDamage = explosiveStrikeDamage;

            if (!string.IsNullOrEmpty(explosiveStrikeCooldown))
            {
                ExplosiveStrikeCooldown = float.Parse(explosiveStrikeCooldown.Replace(",","."), CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(freezingCooldown))
            {
                FreezingCooldown = float.Parse(freezingCooldown.Replace(",", "."), CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(acidCooldown))
            {
                AcidCooldown = float.Parse(acidCooldown.Replace(",", "."), CultureInfo.InvariantCulture);
            }

            FreezingDamage = freezingDamage;
            FreezingAdditionalParameter = freezingAdditionalParameter;
            AcidDamage = acidDamage;

        }
    }
}