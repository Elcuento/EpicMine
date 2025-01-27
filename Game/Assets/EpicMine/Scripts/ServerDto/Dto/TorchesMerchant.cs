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
        public TorchesMerchant(BlackTemple.EpicMine.Core.TorchesMerchant data)
        {
            SelectedTorch = data.SelectedTorch.StaticTorch.Id;

            Torches = new List<Torch>();
            AdTorches = new Dictionary<string, int>();

            foreach (var a in data.AdTorches)
            {
                AdTorches.Add(a.Key, a.Value);
            }

            foreach (var dataTorch in data.Torches)
            {
                Torches.Add(new Torch(dataTorch.StaticTorch.Id,dataTorch.IsCreated));
            }
        }
    }
}
