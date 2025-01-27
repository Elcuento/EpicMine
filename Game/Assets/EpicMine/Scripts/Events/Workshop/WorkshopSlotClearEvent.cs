using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct WorkshopSlotClearEvent
    {
        public WorkshopSlot WorkshopSlot;

        public WorkshopSlotClearEvent(WorkshopSlot workshopSlot)
        {
            WorkshopSlot = workshopSlot;
        }
    }
}