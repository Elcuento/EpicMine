
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public static class DotWeenHelper
    {

        public static void MonsterStrikeShake(Camera camera)
        {
            camera.DOKill(true);

            camera.DOShakePosition(0.4f, new Vector2(1, 1));
        }


        public static void ShakeWallCrash(Camera camera)
        {
            camera.DOKill(true);
            camera.transform.DOShakePosition(0.3f, new Vector3(0.11f, 0.11f, 0.11f), 20);
        }

        public static void ShakeWall(Camera camera, GameObject obj, AttackDamageType damageType, bool critical = false, int multiply = 0, float delay = 0)
        {
            var isShake = false;

            if (critical)
            {
                isShake = true;
                obj.transform.DOKill(true);
                obj.transform.DOShakePosition(0.3f, new Vector3(0.3f, 0.3f, 0.3f), 25);
            }

            if (multiply >= 0)
            {

                if (multiply >= 6)
                {
                    isShake = true;
                    camera.DOKill(true);
                    camera.DOShakePosition(0.3f, new Vector3(0.2f, 0.2f, 0.2f), 25);
                }
                else if (multiply >= 5)
                {
                    isShake = true;
                    camera.DOKill(true);
                    camera.DOShakePosition(0.3f, new Vector3(0.16f, 0.16f, 0.16f), 20);
                }
                else if (multiply >= 4)
                {
                    isShake = true;
                    camera.DOKill(true);
                    camera.DOShakePosition(0.3f, new Vector3(0.11f, 0.11f, 0.11f), 20);
                }
                else if (multiply >= 3)
                {
                    isShake = true;
                    camera.DOKill(true);
                    camera.DOShakePosition(0.3f, new Vector3(0.06f, 0.06f, 0.06f), 20);
                }

            }
            switch (damageType)
            {
                case AttackDamageType.Pickaxe:
                    break;
                case AttackDamageType.Ability:
                    break;
                case AttackDamageType.FireAbility:
                    isShake = true;
                    camera.DOKill(true);
                    camera.DOShakePosition(0.4f, new Vector3(0.2f, 0.2f, 0.2f), 20);
                    break;
                case AttackDamageType.FrostAbility:
                    break;
                case AttackDamageType.AcidAbility:
                    break;
                case AttackDamageType.Item:
                    isShake = true;
                    camera.DOKill(true);
                    camera.DOShakePosition(0.6f, new Vector3(0.4f, 0.4f, 0.4f), 20);
                    break;
            }

            if (!isShake && damageType == AttackDamageType.Pickaxe)
            {
                obj.transform.DOKill(true);
                obj.transform.DOLocalMoveZ(0.2f, 0.05f)
                    .SetEase(Ease.Linear)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetDelay(delay);
            }
        }
    }
}