using System.Collections.Generic;

namespace CommonDLL.Static
{
    public class MonsterAbility
    {
        public string Id ;

        public MonsterAbilityType Type ;

        public float Duration ;

        public float UseTime ;

        public float UseDelay ;

        public float UseDelayRange ;

        public int Percent;

        public List<AttackDamageType> InterruptAbilities ;

        public Dictionary<MonsterAbilityCharacteristicType, float> Characteristics;

        public string Animation ;


    }
}