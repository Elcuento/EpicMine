using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public abstract class TutorialStepAssistantBehaviour : MonoBehaviour
    {
        public TutorialStepIds StepId;
        public TutorialStepIds[] WithIncompletedSteps;
        public TutorialStepIds[] WithCompletedSteps;
    }
}