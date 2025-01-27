using System;

namespace BlackTemple.EpicMine
{
    public struct WorkshopBoostTimeLeftChangeEvent
    {
        public TimeSpan TimeLeft;

        public WorkshopBoostTimeLeftChangeEvent(TimeSpan timeLeft)
        {
            TimeLeft = timeLeft;
        }
    }
}