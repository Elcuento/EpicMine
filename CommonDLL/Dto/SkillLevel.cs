using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class SkillLevel
    {
        public SkillType Type;

        public int Number;

        public SkillLevel(SkillType type, int number)
        {
            Type = type;
            Number = number;
        }
    }
}