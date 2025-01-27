
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneMonsterBuff : MineSceneBuff
    {
        protected MineSceneMonsterSection _monsterSection;
        protected MonsterAbility Ability;

        public virtual void Initialize(MonsterAbility ability, MineSceneMonsterSection monster)
        {
            _monsterSection = monster;
            Ability = ability;
        }

        public override void Clear() { }
    }
}