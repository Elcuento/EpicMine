using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct WorkshopSlotStartMeltingEvent
    {
        public WorkshopSlot WorkshopSlot;

        public WorkshopSlotStartMeltingEvent(WorkshopSlot workshopSlot)
        {
            WorkshopSlot = workshopSlot;
        }
    }
}