using System.Collections.Generic;
using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateAbilities : SendData
    {
        public Dictionary<AbilityType, int> Skills;

        public RequestDataUpdateAbilities(Dictionary<AbilityType, int> items)
        {
            Skills = items;
        }
    }
}