namespace BlackTemple.EpicMine
{
    public struct MineSceneSectionBuffsChangeEvent
    {
        public MineSceneSection Section;

        public MineSceneSectionBuffsChangeEvent(MineSceneSection section)
        {
            Section = section;
        }
    }
}