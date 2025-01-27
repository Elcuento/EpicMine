using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace CommonDLL.Dto
{
    public class Dungeon
    {
        public List<Tier> Tiers;

        public Dungeon()
        {

        }
        public Dungeon(BlackTemple.EpicMine.Core.Dungeon data)
        {
            Tiers = new List<Tier>();
            foreach (var dataTier in data.Tiers)
            {
                var mines = new List<Mine>();
                foreach (var dataTierMine in dataTier.Mines)
                {
                    mines.Add(new Mine()
                    {
                        HardcoreRating = dataTierMine.HardcoreRating,
                        HighScore = dataTierMine.Highscore,
                        IsComplete = dataTierMine.IsComplete,
                        IsGhostAppear = dataTierMine.IsGhostAppear,
                        IsHardcoreOn = dataTierMine.IsHardcoreOn,
                        Number = dataTierMine.Number,
                        Rating = dataTierMine.Rating
                    });
                }

                var dropItems = new List<string>();

                for (var index = 0; index < dataTier.UnlockedDropItems.Count; index++)
                {
                    var dataTierUnlockedDropItem = dataTier.UnlockedDropItems[index];
                    dropItems.Add(dataTierUnlockedDropItem);
                }

                Tiers.Add(new Tier(dataTier.Number,dataTier.IsOpen, mines, dropItems));
            }
        }
    }
}