
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MonstersHelper
    {
        public static float GetMonsterAttackDelayTime(Monster monster)
        {
            var range = monster.AttackDelayRange * 0.01f;
            return Random.Range(monster.AttackDelay - range, monster.AttackDelay + range);
        }

        public static float GetMonsterAbilityDelayTime(MonsterAbility monster)
        {
            var range = monster.UseDelayRange * 0.01f;
            return Random.Range(monster.UseDelay - range, monster.UseDelay + range);
        }


        public static float CalculateDamageOnMonster(Monster monster, AttackDamageType type, float damage)
        {
            monster.Resistance.TryGetValue(type, out var damageVul);

            if (type == AttackDamageType.Item)
            {
                Debug.Log("Tnt use");
                damage = damage * MineLocalConfigs.MonsterTntExtraDamageMultiplier;
            }

            return damage * (1 + damageVul);
        }
    }
}
