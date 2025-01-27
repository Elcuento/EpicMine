using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class AutoMiner
    {
        public AutoMinerSpeedLevel SpeedLevel;

        public AutoMinerCapacityLevel CapacityLevel;

        public Dictionary<string, int> Items;

        public long TimerStart;

        public long TimerEnd;

        public int Tier;

        public bool Started;


        public AutoMiner()
        {
            Items = new Dictionary<string, int>();
        }
        public AutoMiner(BlackTemple.EpicMine.Core.AutoMiner data)
        {
            SpeedLevel = data.SpeedLevel != null ? new AutoMinerSpeedLevel(data.SpeedLevel.Number) : null;
            CapacityLevel = data.CapacityLevel != null ? new AutoMinerCapacityLevel(data.CapacityLevel.Number) : null;
            Items = new Dictionary<string, int>();

            foreach (var dataEarnedResource in data.EarnedResources)
            {
                if (Items.ContainsKey(dataEarnedResource.Key))
                {
                    Items[dataEarnedResource.Key] += dataEarnedResource.Value;
                }
                else
                {
                    Items.Add(dataEarnedResource.Key,dataEarnedResource.Value);
                }
            }

            TimerStart = data.TimeStart;
            TimerEnd = data.TimeEnd;
            Tier = data.Tier;
            Started = data.Started;
        }
    }
}