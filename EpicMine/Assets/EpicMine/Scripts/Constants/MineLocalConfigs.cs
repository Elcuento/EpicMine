using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct MineLocalConfigs
    {
        public const float SectionSize = 5.7f;
        public const float SectionMoveTime = 1f;
        public const float MaxPickaxeHit = 8;
        public const float PickaxePressedCooldown = 0.3f;

        public const float WallSectionMoveDelay = 1.5f;
        public const float WallSectionMonsterDelay = 1.5f;
        public const float WallSectionGhostFlyDelay = 0f;
        public const float WallSectionGhostSpeakDelay = 0.3f;
        public const float OtherSectionMoveDelay = 0.3f;

        public const float MineGhostAppearChance = 50;

        public const float AttackPointSpawnInterval = 0.0015f;

        public const float AttackLineMoveTime = 1.4f;
        public const float AttackLineMoveTimeIncreaseCoefficient = 0.98f;
        public const float VerticalAttackLineMaxYPosition = 2.5f;
        public const float HorizontalAttackLineMaxXPosition = 2.9f;
        public const float HorizontalFieldSize = 5.8f;
        public const float VerticalFieldSize = 5f;

        public const int TorchUseSecCoast = 1;
        public const int TorchUseMomentCoast = 25;
        public const float PickaxeMonsterDamageCoefficient = 0.7f;
        public const float AttackHitFadeTime = 1f;

        public const float WallHealthbarFxTime = 0.65f;
        public const float PickaxeHealthbarFxTime = 0.65f;

        public const int DefaultSectionsCount = 10;
        public const int BossSectionsCount = 7;
        public const int BlacksmithSectionPositionNumber = 5;
        public const int TutorialFindChestMineNumber = 3;
        public const int TutorialFindChestSectionPositionNumber = 4;
        public const int TutorialFindEnchantedChestMineNumber = 4;
        public const int TutorialFindEnchantedChestSectionPositionNumber = 7;
        public const int GodSectionPositionNumber = 4;

        public const float MineDifficultWallHintFirstCoefficient = 30;
        public const float MineDifficultWallHintSecondCoefficient = 50;
        public const float MineDifficultWallBossHintFirstCoefficient = 150;
        public const float MineDifficultWallBossHintSecondCoefficient = 200;

        public const int ExplosiveStrikeAbilityOpenedAtTier = 1;
        public const int FreezingAbilityOpenedAtTier = 3;
        public const int AcidAbilityOpenedAtTier = 5;

        public const int FreezingAbilityMaxStacks = 3;
        public const int AcidAbilityMaxStacks = 5;

        public const float PointSize = 0.65f;
        public const float PointPartSize = 0.325f;
        public const string PointFigure = "1_point";

        public const int MonsterTntExtraDamageMultiplier = 10;

    }
}