using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class AbilityLevel
    {
        public AbilityType Type;

        public int Number;

        public AbilityLevel(AbilityType type, int number)
        {
            Type = type;
            Number = number;
        }
    }
}