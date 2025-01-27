using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public sealed class PerfectMineCompleteDailyTask : DailyTask
    {
        public PerfectMineCompleteDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null) : base(staticTask, dtoTask)
        {
            Subscribe();
        }


        public override void Subscribe()
        {
            base.Subscribe();
            if (!IsCompleted)
                EventManager.Instance.Subscribe<MineCompleteEvent>(OnMineComplete);
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            EventManager.Instance.Unsubscribe<MineCompleteEvent>(OnMineComplete);
        }


        private void OnMineComplete(MineCompleteEvent eventData)
        {
            var isMissedAtLeastOnce = App.Instance.Services.RuntimeStorage.Load<bool>(RuntimeStorageKeys.IsMissedAtLeastOnce);
            if (!isMissedAtLeastOnce)
                Collect();
        }
    }
}