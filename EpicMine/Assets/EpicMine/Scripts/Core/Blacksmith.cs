using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class Blacksmith
    {
        public Pickaxe SelectedPickaxe { get; private set; }

        public List<Pickaxe> Pickaxes { get; }

        public string CurrentAdPickaxe { get; private set; }

        public Dictionary<string, int> AdPickaxes { get; }




        public Blacksmith(CommonDLL.Dto.Blacksmith blacksmithGameDataResponse)
        {
            Pickaxes = new List<Pickaxe>();

            for (var i = 0; i < App.Instance.StaticData.Pickaxes.Count; i++)
            {
                var staticPickaxe = App.Instance.StaticData.Pickaxes[i];
                Pickaxe pickaxe;

                if (blacksmithGameDataResponse.Pickaxes?.Find(x=>x.Id == staticPickaxe.Id) != null)
                {
                    var dtoPickaxe = blacksmithGameDataResponse.Pickaxes.Find(x=>x.Id == staticPickaxe.Id);
                    pickaxe = new Pickaxe(staticPickaxe, dtoPickaxe);

                    if (!string.IsNullOrEmpty(blacksmithGameDataResponse.SelectedPickaxe) && blacksmithGameDataResponse.SelectedPickaxe == pickaxe.StaticPickaxe.Id)
                        SelectedPickaxe = pickaxe;
                }
                else
                {
                    var isHiltFound = i == 0 || staticPickaxe.Type == PickaxeType.Donate;
                    var isCreated = i == 0;
                    pickaxe = new Pickaxe(staticPickaxe, isCreated, isHiltFound);
                }

                Pickaxes.Add(pickaxe);
            }

            if (SelectedPickaxe == null)
                SelectedPickaxe = Pickaxes.FirstOrDefault();

            AdPickaxes = blacksmithGameDataResponse.AdPickaxes ?? new Dictionary<string, int>();

            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnAdRewardedVideoCompleted;
            App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnAdRewardedVideoCompleted;
        }


        public void Select(Pickaxe pickaxe)
        {
            if (!pickaxe.IsCreated)
                return;

            SelectedPickaxe = pickaxe;
            
            var selectEvent = new PickaxeSelectEvent(SelectedPickaxe);
            EventManager.Instance.Publish(selectEvent);
        }

        public void SetAdPickaxe(string id)
        {
            CurrentAdPickaxe = id;
        }


        private void OnAdRewardedVideoCompleted(AdSource adSource, bool isShowed, string rewardId, int rewardAmount)
        {
            if (!isShowed || adSource != AdSource.UnlockPickaxe)
                return;

            int currentVal;
            AdPickaxes.TryGetValue(CurrentAdPickaxe, out currentVal);

            currentVal++;
            AdPickaxes[CurrentAdPickaxe] = currentVal;

            var pickaxe = Pickaxes.FirstOrDefault(p => p.StaticPickaxe.Id == CurrentAdPickaxe);
            if (pickaxe != null && pickaxe.StaticPickaxe.Cost <= currentVal)
            {
                pickaxe.Create();
            }
            else
                EventManager.Instance.Publish(new AdPickaxesChangeEvent(AdPickaxes));
        }
    }
}