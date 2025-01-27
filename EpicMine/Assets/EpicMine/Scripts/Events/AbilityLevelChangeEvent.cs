using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct AbilityLevelChangeEvent
    {
        public AbilityLevel AbilityLevel;

        public AbilityLevelChangeEvent(AbilityLevel abilityLevel)
        {
            AbilityLevel = abilityLevel;
        }
    }
}