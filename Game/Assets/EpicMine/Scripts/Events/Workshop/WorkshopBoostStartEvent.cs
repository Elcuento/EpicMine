

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct WorkshopBoostStartEvent
    {
        public BuffValueType BoostType;

        public WorkshopBoostStartEvent(BuffValueType boostType)
        {
            BoostType = boostType;
        }
    }
}