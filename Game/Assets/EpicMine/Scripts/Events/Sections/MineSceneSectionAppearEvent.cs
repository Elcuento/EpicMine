namespace BlackTemple.EpicMine
{
    public struct MineSceneSectionAppearEvent
    {
        public MineSceneSection Section;

        public MineSceneSectionAppearEvent(MineSceneSection section)
        {
            Section = section;
        }
    }
}