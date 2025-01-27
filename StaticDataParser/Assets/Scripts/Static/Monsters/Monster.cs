using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class Monster
    {
        public string Id { get; }

        public List<string> Abilities { get; }

        public int HealthMin { get; }

        public float HealthMaxPercentOffset { get; }

        public float AttackTime { get; }

        public float AttackDelay { get; }

        public float AttackDelayRange { get; }

        public int Damage { get; }

        public Dictionary<AttackDamageType, float> Resistance { get; }

        public MonsterType Type;

        public List<string> Skins { get; }

        public List<DropChance> DropItems { get; }

        public List<DropChance> DropCurrency { get; }

        public Monster(string id, string abilities, string healthRange, int damage, float attackDelay, float attackDelayRange, float attackTime, string skins, string dropItems, string dropCurrency, MonsterType type, string resistance)
        {
            Id = id;
            Type = type;

            Resistance = Extensions.GetDictionaryBySplitKeyValuePair<AttackDamageType, float>(resistance, '#');

            Abilities = string.IsNullOrEmpty(abilities) ? new List<string>() : abilities.Split('#').ToList();

            var healthSplit = healthRange.Split(';');
            HealthMin = int.Parse(healthSplit[0]);
            HealthMaxPercentOffset = int.Parse(healthSplit[1]) * 0.01f;

            Damage = damage;
            AttackDelay = attackDelay;
            AttackDelayRange = attackDelayRange;
            AttackTime = attackTime;

            Skins = string.IsNullOrEmpty(skins) ? new List<string>() : skins.Split(';').ToList();

            DropItems = new List<DropChance>();
            DropCurrency = new List<DropChance>();

            // Items

            if (dropItems.Length > 0)
            {
                var dropSplit = dropItems.Split('#');
                foreach (var dropValue in dropSplit)
                {
                    var dropPack = dropValue.Split(';');

                    var item = dropPack[0];
                    var dropMin = int.Parse(dropPack[1]);
                    var dropMax = int.Parse(dropPack[2]);
                    var chance = int.Parse(dropPack[3]);

                    var dropC = new DropChance(item, EntityType.Item, chance, chance, dropMin, dropMax);
                    DropItems.Add(dropC);
                }
            }

            // Currency

            if (dropCurrency.Length > 0)
            {
                var dropCurrencySplit = dropCurrency.Split('#');
                foreach (var dropValue in dropCurrencySplit)
                {
                    var dropPack = dropValue.Split(';');

                    var item = dropPack[0];
                    var dropMin = int.Parse(dropPack[1]);
                    var dropMax = int.Parse(dropPack[2]);
                    var chance = int.Parse(dropPack[3]);

                    var dropC = new DropChance(item, EntityType.CurrencyGold, chance, chance, dropMin, dropMax);
                    DropCurrency.Add(dropC);
                }
            }
        }
    }
}