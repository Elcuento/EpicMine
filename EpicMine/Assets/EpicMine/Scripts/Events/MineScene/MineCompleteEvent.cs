namespace BlackTemple.EpicMine
{
    public struct MineCompleteEvent
    {
        public Core.Mine Mine;

        public MineCompleteEvent(Core.Mine mine)
        {
            Mine = mine;
        }
    }
}