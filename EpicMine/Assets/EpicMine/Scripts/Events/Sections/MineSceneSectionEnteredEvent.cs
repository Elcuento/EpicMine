namespace BlackTemple.EpicMine
{
    public struct MineSceneSectionEnteredEvent
    {
        public MineSceneSection Section;

        public MineSceneSectionEnteredEvent(MineSceneSection section)
        {
            Section = section;
        }
    }
}