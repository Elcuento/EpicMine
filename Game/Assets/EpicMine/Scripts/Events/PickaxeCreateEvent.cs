using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct PickaxeCreateEvent
    {
        public Pickaxe Pickaxe;

        public PickaxeCreateEvent(Pickaxe pickaxe)
        {
            Pickaxe = pickaxe;
        }
    }
}