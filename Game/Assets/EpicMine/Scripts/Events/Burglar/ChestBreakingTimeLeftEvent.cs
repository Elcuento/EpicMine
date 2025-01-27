using System;
using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct ChestBreakingTimeLeftEvent
    {
        public Chest Chest;
        public TimeSpan TimeLeft;

        public ChestBreakingTimeLeftEvent(Chest chest, TimeSpan timeLeft)
        {
            Chest = chest;
            TimeLeft = timeLeft;
        }
    }
}