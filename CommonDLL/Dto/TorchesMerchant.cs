using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class TorchesMerchant
    {
        public string SelectedTorch;

        public List<Torch> Torches;

        public Dictionary<string, int> AdTorches;

        public TorchesMerchant()
        {
            Torches = new List<Torch>();
            AdTorches = new Dictionary<string, int>();
        }
    }
}
