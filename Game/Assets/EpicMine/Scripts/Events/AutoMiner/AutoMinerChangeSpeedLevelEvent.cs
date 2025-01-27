
using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct AutoMinerChangeSpeedLevelEvent
    {
        public AutoMinerSpeedLevel Level;

        public AutoMinerChangeSpeedLevelEvent(AutoMinerSpeedLevel level)
        {
            Level = level;
        }
    }
}