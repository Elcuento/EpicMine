using System.Collections.Generic;


namespace BlackTemple.EpicMine
{
    public struct PvpInviteEnableEvent
    {
        public bool IsEnable;

        public PvpInviteEnableEvent(bool enable)
        {
            IsEnable = enable;
        }
    }
}