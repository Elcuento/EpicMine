using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class DropChance
    {
        public string Id { get; }

        public EntityType Type { get; }

        public float Chance { get; }

        public float ChanceMax { get; }

        public int Count { get; }

        public int CountMax { get; }

        public DropChance(string id, EntityType type, float chance, float chanceMax, int count, int countMax)
        {
            Id = id;
            Type = type;
            Chance = chance;
            ChanceMax = chanceMax < chance ? chance : chanceMax;
            Count = count <= 0 ? 1 : count;
            CountMax = countMax <= count ? count : countMax;
        }
    }
}