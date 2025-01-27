using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct ChestAddedEvent
    {
        public Chest Chest;

        public ChestAddedEvent(Chest chest)
        {
            Chest = chest;
        }
    }
}