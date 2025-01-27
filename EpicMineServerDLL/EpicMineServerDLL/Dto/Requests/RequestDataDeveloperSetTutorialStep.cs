using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataDeveloperSetTutorialStep : SendData
    {
        public TutorialStepIds Id;

        public RequestDataDeveloperSetTutorialStep(TutorialStepIds id)
        {
            Id = id;
        }

    }
 
}