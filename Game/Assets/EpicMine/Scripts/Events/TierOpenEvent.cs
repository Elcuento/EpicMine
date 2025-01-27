using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct TierOpenEvent
    {
        public Tier Tier;

        public TierOpenEvent(Tier tier)
        {
            Tier = tier;
        }
    }
}