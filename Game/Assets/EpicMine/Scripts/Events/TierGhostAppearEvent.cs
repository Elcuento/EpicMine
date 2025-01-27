using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct TierGhostAppearEvent
    {
        public bool IsSpeak;

        public TierGhostAppearEvent(bool isSpeak)
        {
            IsSpeak = isSpeak;
        }
    }
}