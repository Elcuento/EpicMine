namespace BlackTemple.EpicMine
{
    public struct AutoMinerChangeEvent
    {
        public int CompleteAmount;

        public AutoMinerChangeEvent(int completeAmount)
        {
            CompleteAmount = completeAmount;
        }
    }
}