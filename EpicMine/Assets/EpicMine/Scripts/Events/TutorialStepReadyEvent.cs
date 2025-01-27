namespace BlackTemple.EpicMine
{
    public struct TutorialStepReadyEvent
    {
        public TutorialStepBase Step;

        public TutorialStepReadyEvent(TutorialStepBase step)
        {
            Step = step;
        }
    }
}