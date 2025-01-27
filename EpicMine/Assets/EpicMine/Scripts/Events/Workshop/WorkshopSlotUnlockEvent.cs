using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct WorkshopSlotUnlockEvent
    {
        public WorkshopSlot WorkshopSlot;

        public WorkshopSlotUnlockEvent(WorkshopSlot workshopSlot)
        {
            WorkshopSlot = workshopSlot;
        }
    }
}