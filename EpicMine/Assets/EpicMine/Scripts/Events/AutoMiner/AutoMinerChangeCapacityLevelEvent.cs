namespace BlackTemple.EpicMine
{
    public struct AutoMinerChangeCapacityLevelEvent
    {
        public Core.AutoMinerCapacityLevel Level;

        public AutoMinerChangeCapacityLevelEvent(Core.AutoMinerCapacityLevel level)
        {
            Level = level;
        }
    }
}