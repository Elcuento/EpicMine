namespace BlackTemple.EpicMine
{
    public struct MineSceneSectionBuffAddEvent
    {
        public MineSceneSection Section;

        public MineSceneSectionBuff Buff;

        public MineSceneSectionBuffAddEvent(MineSceneSection section, MineSceneSectionBuff buff)
        {
            Section = section;
            Buff = buff;
        }
    }
}