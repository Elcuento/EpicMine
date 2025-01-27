namespace BlackTemple.EpicMine
{
    public struct MineEnterEvent
    {
        public Core.Mine Mine;

        public MineEnterEvent(Core.Mine mine)
        {
            Mine = mine;
        }
    }
}