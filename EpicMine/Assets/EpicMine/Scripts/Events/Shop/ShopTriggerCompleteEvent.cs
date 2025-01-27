using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;

namespace BlackTemple.EpicMine
{
    public struct ShopTriggerCompleteEvent
    {
        public ShopTrigger ShopTrigger;

        public ShopTriggerCompleteEvent(ShopTrigger shopTrigger)
        {
            ShopTrigger = shopTrigger;
        }
    }
}