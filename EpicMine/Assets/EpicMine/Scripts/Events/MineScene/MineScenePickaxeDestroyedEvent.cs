namespace BlackTemple.EpicMine
{
    public struct MineScenePickaxeDestroyedEvent
    {
        public MineSceneSection Section;

        public MineScenePickaxeDestroyedEvent(MineSceneSection section)
        {
            Section = section;
        }
    }
}