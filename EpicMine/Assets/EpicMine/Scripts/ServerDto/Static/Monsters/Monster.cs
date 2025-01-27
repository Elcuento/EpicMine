using System.Collections.Generic;

namespace CommonDLL.Static
{
    public class Monster
    {
        public string Id ;

        public List<string> Abilities ;

        public int HealthMin ;

        public float HealthMaxPercentOffset ;

        public float AttackTime ;

        public float AttackDelay ;

        public float AttackDelayRange ;

        public int Damage ;

        public Dictionary<AttackDamageType, float> Resistance ;

        public MonsterType Type;

        public List<string> Skins ;

        public List<DropChance> DropItems ;

        public List<DropChance> DropCurrency ;
    }
}