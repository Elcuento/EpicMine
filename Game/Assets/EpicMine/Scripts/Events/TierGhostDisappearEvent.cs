

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct TierGhostDisappearEvent
    {
        public GhostActionType Action;

        public TierGhostDisappearEvent(GhostActionType action)
        {
            Action = action;
        }
    }
}