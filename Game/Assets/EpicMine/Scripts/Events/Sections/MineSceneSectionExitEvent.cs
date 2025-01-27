namespace BlackTemple.EpicMine
{
    public struct MineSceneSectionExitEvent
    {
        public MineSceneSection Section;

        public MineSceneSectionExitEvent(MineSceneSection section)
        {
            Section = section;
        }
    }
}