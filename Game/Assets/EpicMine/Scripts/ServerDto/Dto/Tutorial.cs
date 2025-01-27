using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class Tutorial
    {
        public TutorialStepIds LastCompleteStep;

        public Tutorial()
        {

        }
        public Tutorial(int lastCompleteStep)
        {
            LastCompleteStep = (TutorialStepIds)lastCompleteStep;
        }
    }
}