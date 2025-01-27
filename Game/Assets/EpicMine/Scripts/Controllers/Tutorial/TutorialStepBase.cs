using BlackTemple.Common;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public abstract class TutorialStepBase
    {
        public TutorialStepIds Id { get; }

        public bool IsReady { get; private set; }

        public bool IsComplete { get; private set; }


        protected TutorialStepBase(TutorialStepIds id, bool isComplete)
        {
            Id = id;
            IsComplete = isComplete;
        }


        public abstract void CheckReady();

        public virtual void Clear()
        {
            IsReady = false;
            IsComplete = false;
        }

        protected void SetReady()
        {
            if (IsComplete)
                return;

            IsReady = true;

            var readyEvent = new TutorialStepReadyEvent(this);
            EventManager.Instance.Publish(readyEvent);

            OnReady();
        }

        protected abstract void OnReady();


        protected void SetComplete()
        {
            IsReady = false;
            IsComplete = true;

            var completeEvent = new TutorialStepCompleteEvent(this);
            EventManager.Instance.Publish(completeEvent);

            OnComplete();

            App.Instance.Player.SetTutorialStep((int)Id);

            App.Instance.Services.AnalyticsService.SetTutorialStepComplete((int)Id);
        }

        public void SetForceComplete()
        {
            SetComplete();
        }
        
        protected abstract void OnComplete();
    }
}