using System.Collections.Generic;
using System.Linq;

namespace BlackTemple.EpicMine.Core
{
    public class Dungeon
    {
        public List<Tier> Tiers { get; }

        public Tier LastOpenedTier { get { return Tiers.LastOrDefault(t => t.IsOpen); } }

        public int LastOpenTierNumber { get { return Tiers.LastOrDefault(t => t.IsOpen)?.Number ?? 0 ;} }


        public Dungeon(CommonDLL.Dto.Dungeon dungeonGameDataResponse)
        {
            Tiers = new List<Tier>();

            for (var i = 0; i < App.Instance.StaticData.Tiers.Count; i++)
            {
                Tier tier;

                if (dungeonGameDataResponse.Tiers != null && dungeonGameDataResponse.Tiers.Count > i)
                {
                    var tierData = dungeonGameDataResponse.Tiers[i];
                    tier = new Tier(i, tierData);
                }
                else
                    tier = new Tier(i);

                Tiers.Add(tier);
            }
        }





    }
}