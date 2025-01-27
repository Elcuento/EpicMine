using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct BurglarChestOpenedEvent
    {
        public Chest Chest;

        public BurglarChestOpenedEvent(Chest chest)
        {
            Chest = chest;
        }
    }
}