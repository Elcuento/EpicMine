using System.Collections.Generic;
using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateMinerLevels : SendData
    {
        public Dictionary<AutoMinerUpgradeType, int> Skills;

        public RequestDataUpdateMinerLevels(Dictionary<AutoMinerUpgradeType, int> items)
        {
            Skills = items;
        }
    }
}