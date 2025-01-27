
using CommonDLL.Static;
using AbilityLevel = BlackTemple.EpicMine.Core.AbilityLevel;

namespace BlackTemple.EpicMine
{
    public struct PvpArenaOnFindChest
    {
        public PvpChestType Type;

        public PvpArenaOnFindChest(PvpChestType type)
        {
            Type = type;
        }
    }
}