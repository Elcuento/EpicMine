using System.Collections.Generic;
using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackAreaNoPointsWall : MineSceneAttackArea
    {
        public override void Initialize(MineSceneSection section, bool isHorizontalAttackLine, float moveTime)
        {
            base.Initialize(section, isHorizontalAttackLine, moveTime);

            Section = section;
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
                AttackPointHitType>(), 0, false));

            OnHit?.Invoke();
        }

        protected override void OnChangeDirection()
        {
          
        }

    }

}

