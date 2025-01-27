namespace BlackTemple.EpicMine
{
    public struct MineSceneSectionPassedEvent
    {
        public MineSceneSection Section;

        public float Delay;

        public MineSceneSectionPassedEvent(MineSceneSection section, float delay = 0)
        {
            Section = section;
            Delay = delay;
        }
    }
}