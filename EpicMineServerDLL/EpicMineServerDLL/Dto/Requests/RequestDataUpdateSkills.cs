using System.Collections.Generic;
using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateSkills : SendData
    {
        public Dictionary<SkillType, int> Skills;

        public RequestDataUpdateSkills(Dictionary<SkillType, int> items)
        {
            Skills = items;
        }
    }
}