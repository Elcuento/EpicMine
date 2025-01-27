using System.Collections.Generic;
using BlackTemple.Common;

namespace BlackTemple.EpicMine.Static
{
    public class MonsterAbility
    {
        public string Id { get; }

        public MonsterAbilityType Type { get; }

        public float Duration { get; }

        public float UseTime { get; }

        public float UseDelay { get; }

        public float UseDelayRange { get; }

        public List<AttackDamageType> InterruptAbilities { get; }

        public Dictionary<MonsterAbilityCharacteristicType, float> Characteristics;

        public int Percent { get; }

        public string Animation { get; }

        public MonsterAbility(string id, MonsterAbilityType type, int percent, string characteristics, float duration, float useTime, float useDelay, float useDelayRange, string interruptAbilities, string animation)
        {
            Id = id;
            Type = type;
            Duration = duration;
            UseTime = useTime;
            Percent = percent;
            UseDelay = useDelay;
            UseDelayRange = useDelayRange;
            InterruptAbilities = Extensions.SplitToList<AttackDamageType>(interruptAbilities,";");
            Characteristics = Extensions.GetDictionaryBySplitKeyValuePair<MonsterAbilityCharacteristicType, float>(characteristics,'#');
            Animation = animation;

        }
    }
}