

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct MineSceneWallSectionDamageEvent
    {
        public MineSceneAttackSection HealthHandler;

        public MineSceneAttackPoint AttackPoint;

        public float Damage;

        public bool IsCritical;

        public bool IsMissed;

        public bool IsImmunity;

        public AttackDamageType Source;

        public MineSceneWallSectionDamageEvent(MineSceneAttackSection healthHandler, 
            MineSceneAttackPoint attackPoint, 
            float damage,
            bool isCritical,
            bool isMissed,
            AttackDamageType source,
            bool isImmunity = false
            )
        {
            HealthHandler = healthHandler;
            AttackPoint = attackPoint;
            Damage = damage;
            IsCritical = isCritical;
            IsMissed = isMissed;
            Source = source;
            IsImmunity = isImmunity;
        }

    }
}