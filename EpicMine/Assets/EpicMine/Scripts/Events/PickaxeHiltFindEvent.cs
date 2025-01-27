using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct PickaxeHiltFindEvent
    {
        public Pickaxe Pickaxe;

        public PickaxeHiltFindEvent(Pickaxe pickaxe)
        {
            Pickaxe = pickaxe;
        }
    }
}