using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct AdPickaxesChangeEvent
    {
        public Dictionary<string, int> AdPickaxes;

        public AdPickaxesChangeEvent(Dictionary<string, int> adPickaxes)
        {
            AdPickaxes = adPickaxes;
        }
    }
}