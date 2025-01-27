namespace BlackTemple.EpicMine
{
    public struct WorkshopSlotCompleteEvent
    {
        public Core.WorkshopSlot WorkshopSlot;

        public WorkshopSlotCompleteEvent(Core.WorkshopSlot workshopSlot)
        {
            WorkshopSlot = workshopSlot;
        }
    }
}