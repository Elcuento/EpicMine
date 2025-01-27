using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct ChestStartBreakingEvent
    {
        public Chest Chest;

        public ChestStartBreakingEvent(Chest chest)
        {
            Chest = chest;
        }
    }
}