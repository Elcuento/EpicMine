namespace BlackTemple.EpicMine
{
    public struct MineSceneSectionReadyEvent
    {
        public MineSceneSection Section;

        public MineSceneSectionReadyEvent(MineSceneSection section)
        {
            Section = section;
        }
    }
}