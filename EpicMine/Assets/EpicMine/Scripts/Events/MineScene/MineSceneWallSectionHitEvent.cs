using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct MineSceneWallSectionHitEvent
    {
        public MineSceneSection Section;
        public Dictionary<MineSceneAttackPointCoordinates, AttackPointHitType> AttackPoints;

        public bool IsMiss;
        public int Combo;

        public MineSceneWallSectionHitEvent(MineSceneSection section, Dictionary<MineSceneAttackPointCoordinates, AttackPointHitType> attackPoints, int combo, bool isMiss)
        {
            Section = section;
            AttackPoints = attackPoints;
            Combo = combo;
            IsMiss = isMiss;
        }
    }
}