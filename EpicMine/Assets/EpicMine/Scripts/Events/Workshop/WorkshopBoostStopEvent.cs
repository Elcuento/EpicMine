

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct WorkshopBoostStopEvent
    {
        public BuffValueType BoostType;

        public WorkshopBoostStopEvent(BuffValueType boostType)
        {
            BoostType = boostType;
        }
    }
}