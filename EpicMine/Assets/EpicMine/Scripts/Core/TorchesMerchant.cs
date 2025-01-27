using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class TorchesMerchant
    {
        public Torch SelectedTorch { get; private set; }

        public List<Torch> Torches { get; }

        public string CurrentAdTorch { get; private set; }

        public Dictionary<string, int> AdTorches { get; }

     


        public TorchesMerchant(CommonDLL.Dto.TorchesMerchant torchesGameDataResponse)
        {
            Torches = new List<Torch>();

            for (var i = 0; i < App.Instance.StaticData.Torches.Count; i++)
            {
                var staticTorch = App.Instance.StaticData.Torches[i];
                Torch torch;

                if (torchesGameDataResponse.Torches?.Find(x=>x.Id == staticTorch.Id) != null)
                {
                    var dtoTorches = torchesGameDataResponse.Torches.Find(x=>x.Id == staticTorch.Id);
                    torch = new Torch(staticTorch, dtoTorches);

                    if (!string.IsNullOrEmpty(torchesGameDataResponse.SelectedTorch)
                        && torchesGameDataResponse.SelectedTorch == torch.StaticTorch.Id)
                        SelectedTorch = torch;
                }
                else
                {
                    var isCreated = i == 0;
                    torch = new Torch(staticTorch, isCreated);
                }


                Torches.Add(torch);



            }

            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnAdRewardedVideoCompleted;
            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnAdRewardedVideoCompleted;

            AdTorches = torchesGameDataResponse.AdTorches ?? new Dictionary<string, int>();

            if (SelectedTorch == null)
                SelectedTorch = Torches.FirstOrDefault();

        }

        public void Select(Torch torch)
        {
          //  App.Instance.Services.LogService.Log(torch.StaticTorch.Id);
            if (!torch.IsCreated)
                return;

            SelectedTorch = torch;
            EventManager.Instance.Publish(new TorchSelectEvent(SelectedTorch));
        }

        public void SetAdTorch(string id)
        {
            CurrentAdTorch = id;
        }

        private void OnAdRewardedVideoCompleted(AdSource adSource, bool isShowed, string rewardId, int rewardAmount)
        { 
            if (!isShowed || adSource != AdSource.UnlockTorch)
                return;

            int currentVal;

            AdTorches.TryGetValue(CurrentAdTorch, out currentVal);

            currentVal++;
            AdTorches[CurrentAdTorch] = currentVal;

            var torch = Torches.FirstOrDefault(p => p.StaticTorch.Id == CurrentAdTorch);
            if (torch != null && torch.StaticTorch.Cost <= currentVal)
            {
                torch.Create();
            }
            else
                EventManager.Instance.Publish(new AdTorchesChangeEvent(AdTorches));
        }
    }
}