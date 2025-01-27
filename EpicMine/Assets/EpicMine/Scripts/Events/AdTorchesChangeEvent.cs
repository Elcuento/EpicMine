using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public struct AdTorchesChangeEvent
    {
        public Dictionary<string, int> AdTorches;

        public AdTorchesChangeEvent(Dictionary<string, int> adTorchest)
        {
            AdTorches = adTorchest;
        }
    }
}