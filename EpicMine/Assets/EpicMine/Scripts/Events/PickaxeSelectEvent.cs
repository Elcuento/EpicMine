using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct PickaxeSelectEvent
    {
        public Pickaxe Pickaxe;

        public PickaxeSelectEvent(Pickaxe pickaxe)
        {
            Pickaxe = pickaxe;
        }
    }
}