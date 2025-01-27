using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class Burglar
    {
        public List<Chest> Chests;

        public Burglar()
        {
            Chests = new List<Chest>();
        }

        public Burglar(BlackTemple.EpicMine.Core.Burglar data)
        {
            Chests = new List<Chest>();

            foreach (var dataChest in data.Chests)
            {
                Chests.Add(new Chest()
                {
                    Id = dataChest.Id,
                    Level = dataChest.Level,
                    StartBreakingTime = dataChest.StartBreakingTime,
                    Type = dataChest.Type
                });
            }
        }
    }
}