using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public struct AttackPointsLocalConfig
    {
        public const string GreenDestroyInnerAnimationName = "AttackPointDestroyInnerGreen";
        public const string RedDestroyInnerAnimationName = "AttackPointDestroyInnerRed";
        public const string YellowDestroyInnerAnimationName = "AttackPointDestroyInnerYellow";

        public const float InnerFirstStepDuration = 0.2f;
        public const float InnerSecondStepDuration = 0.2f;
        public const float OuterFirstStepDuration = 0.1f;
        public const float OuterSecondStepDuration = 0.2f;
        public const float OuterThirdStepDuration = 0.1f;
    }
}