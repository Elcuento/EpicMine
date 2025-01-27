using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct TierUnlockDropItemEvent
    {
        public Tier Tier;
        public string ItemId;

        public TierUnlockDropItemEvent(Tier tier, string itemId)
        {
            Tier = tier;
            ItemId = itemId;
        }
    }
}