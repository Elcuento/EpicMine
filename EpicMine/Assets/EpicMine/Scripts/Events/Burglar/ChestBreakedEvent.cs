using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct ChestBreakedEvent
    {
        public Chest Chest;

        public ChestBreakedEvent(Chest chest)
        {
            Chest = chest;
        }
    }
}