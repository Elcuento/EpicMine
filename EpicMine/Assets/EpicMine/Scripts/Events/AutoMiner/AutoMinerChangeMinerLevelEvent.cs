namespace BlackTemple.EpicMine
{
    public struct AutoMinerChangeMinerLevelEvent
    {
        public int Level;

        public AutoMinerChangeMinerLevelEvent(int lvl)
        {
            Level = lvl;
        }
    }
}