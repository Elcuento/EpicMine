using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct WorkshopSlotMeltingTimeLeftChangeEvent
    {
        public WorkshopSlot WorkshopSlot;

        public WorkshopSlotMeltingTimeLeftChangeEvent(WorkshopSlot workshopSlot)
        {
            WorkshopSlot = workshopSlot;
        }
    }
}