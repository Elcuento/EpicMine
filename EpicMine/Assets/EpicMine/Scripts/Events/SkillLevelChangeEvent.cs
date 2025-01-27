using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct SkillLevelChangeEvent
    {
        public SkillLevel SkillLevel;

        public SkillLevelChangeEvent(SkillLevel skillLevel)
        {
            SkillLevel = skillLevel;
        }
    }
}