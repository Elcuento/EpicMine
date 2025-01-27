using BlackTemple.EpicMine.Core;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public struct OpenChestEvent
    {
        public ChestType Type;

        public OpenChestEvent(ChestType type)
        {
            Type = type;
        }

    }
}