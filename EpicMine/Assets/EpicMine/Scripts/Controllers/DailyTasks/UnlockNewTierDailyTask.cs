using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public sealed class UnlockNewTierDailyTask : DailyTask
    {
        public UnlockNewTierDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null) : base(staticTask, dtoTask)
        {
            Subscribe();
        }


        public override void Subscribe()
        {
            base.Subscribe();
            if (!IsCompleted)
                EventManager.Instance.Subscribe<TierOpenEvent>(OnTierOpen);
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            EventManager.Instance.Unsubscribe<TierOpenEvent>(OnTierOpen);
        }


        private void OnTierOpen(TierOpenEvent eventData)
        {
            Collect();
        }
    }
}