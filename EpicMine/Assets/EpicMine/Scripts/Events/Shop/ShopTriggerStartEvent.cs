using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;

namespace BlackTemple.EpicMine
{
    public struct ShopTriggerStartEvent
    {
        public ShopTrigger ShopTrigger;

        public ShopTriggerStartEvent(ShopTrigger shopTrigger)
        {
            ShopTrigger = shopTrigger;
        }
    }
}