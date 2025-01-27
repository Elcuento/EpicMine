using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateTutorialStepId : SendData
    {
        public TutorialStepIds StepId;
        
        public RequestDataUpdateTutorialStepId(TutorialStepIds step)
        {
            StepId = step;
        }
    }
}