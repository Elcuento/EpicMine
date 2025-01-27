using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct MineChangeEvent
    {
        public Mine Mine;

        public MineChangeEvent(Mine mine)
        {
            Mine = mine;
        }
    }
}