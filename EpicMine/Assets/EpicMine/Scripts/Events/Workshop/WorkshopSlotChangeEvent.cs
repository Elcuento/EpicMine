using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct WorkshopSlotChangeEvent
    {
        public WorkshopSlot WorkshopSlot;

        public WorkshopSlotChangeEvent(WorkshopSlot workshopSlot)
        {
            WorkshopSlot = workshopSlot;
        }
    }
}