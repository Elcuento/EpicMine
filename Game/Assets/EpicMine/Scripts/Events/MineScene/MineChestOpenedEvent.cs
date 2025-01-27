

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct MineChestOpenedEvent
    {
        public ChestType ChestType;
        public int Level;

        public MineChestOpenedEvent(ChestType chestType, int level)
        {
            ChestType = chestType;
            Level = level;
        }
    }
}