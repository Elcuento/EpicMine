using System.Collections.Generic;
using BlackTemple.Common;
using UnityEngine.EventSystems;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackAreaNoPointsMonster : MineSceneAttackArea
    {
        public new MineSceneMonsterSection Section { get; protected set; }

        public override void Initialize(MineSceneSection section, bool isHorizontalAttackLine, float moveTime)
        {
            base.Initialize(section, isHorizontalAttackLine, moveTime);

            Section = section as MineSceneMonsterSection;
        }

        public override void OnClick(bool force = false)
        {
            if (!force)
            {
                if (!Section.Hero.Pickaxe.IsReadyClick || _animCooldown > 0)
                    return;

                _animCooldown = MineLocalConfigs.PickaxePressedCooldown;

            }

            EventManager.Instance.Publish(new MineSceneWallSectionHitEvent(Section, new Dictionary<MineSceneAttackPointCoordinates,
                AttackPointHitType>(), 0, Section.Monster.IsAway));

            OnHit?.Invoke();


        }


        protected override void OnChangeDirection()
        {
        }
        
    }

}

