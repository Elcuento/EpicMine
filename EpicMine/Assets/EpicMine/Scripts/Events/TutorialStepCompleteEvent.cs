namespace BlackTemple.EpicMine
{
    public struct TutorialStepCompleteEvent
    {
        public TutorialStepBase Step;

        public TutorialStepCompleteEvent(TutorialStepBase step)
        {
            Step = step;
        }
    }
}