using System;
using System.Collections.Generic;

namespace BlackTemple.EpicMine.Dto
{
    public struct Workshop
    {
        public DateTime? BoostStartTime;

        public List<WorkshopSlot> Slots;

        public Workshop(DateTime boostStartTime, List<WorkshopSlot> slots)
        {
            BoostStartTime = boostStartTime;
            Slots = slots;
        }
    }
}