namespace BlackTemple.EpicMine
{
    public struct MineSceneTorchUseEvent
    {
        public bool IsStart;

        public MineSceneTorchUseEvent(bool isStart)
        {
            IsStart = isStart;
        }
    }
}