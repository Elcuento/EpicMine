using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public sealed class UnlockNewMineDailyTask : DailyTask
    {
        public UnlockNewMineDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null) : base(staticTask, dtoTask)
        {
            Subscribe();
        }


        public override void Subscribe()
        {
            base.Subscribe();
            if (!IsCompleted)
            {
                EventManager.Instance.Subscribe<MineCompleteEvent>(OnMineComplete);
                EventManager.Instance.Subscribe<TierOpenEvent>(OnTierOpen);
            }
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            EventManager.Instance.Unsubscribe<MineCompleteEvent>(OnMineComplete);
            EventManager.Instance.Unsubscribe<TierOpenEvent>(OnTierOpen);
        }


        private void OnTierOpen(TierOpenEvent eventData)
        {
            Collect();
        }

        private void OnMineComplete(MineCompleteEvent eventData)
        {
            if (!eventData.Mine.IsLast)
                Collect();
        }
    }
}