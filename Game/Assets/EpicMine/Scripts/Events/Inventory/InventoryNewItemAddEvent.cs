using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;

namespace BlackTemple.EpicMine
{
    public struct InventoryNewItemAddEvent
    {
        public Item Item;

        public InventoryNewItemAddEvent(Item item)
        {
            Item = item;
        }
    }
}