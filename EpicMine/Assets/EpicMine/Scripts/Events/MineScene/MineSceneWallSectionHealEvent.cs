namespace BlackTemple.EpicMine
{
    public class MineSceneWallSectionHealEvent
    {
        public MineSceneAttackSection Section;

        public MineSceneWallSectionHealEvent(MineSceneAttackSection section)
        {
            Section = section;
        }
    }
}