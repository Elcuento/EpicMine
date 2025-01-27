namespace BlackTemple.EpicMine
{
    public struct MineSceneSectionEndActionEvent
    {
        public MineSceneSection Section;

        public MineSceneSectionEndActionEvent(MineSceneSection section)
        {
            Section = section;
        }
    }
}