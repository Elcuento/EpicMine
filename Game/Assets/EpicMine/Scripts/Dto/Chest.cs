using System;
using CommonDLL.Static;


namespace BlackTemple.EpicMine.Dto
{
    public struct Chest
    {
        public ChestType Type;

        public int Level;

        public DateTime? StartBreakingTime;

        public Chest(ChestType type, int level, DateTime? startBreakingTime)
        {
            Type = type;
            Level = level;
            StartBreakingTime = startBreakingTime;
        }

        public Chest(ChestType type, int level) : this()
        {
            Type = type;
            Level = level;
        }
    }
}