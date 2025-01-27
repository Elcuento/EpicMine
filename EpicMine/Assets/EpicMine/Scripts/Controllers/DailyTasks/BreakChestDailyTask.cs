using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public sealed class BreakChestDailyTask : DailyTask
    {
        public BreakChestDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null) : base(staticTask, dtoTask)
        {
            Subscribe();
        }

        public override void Subscribe()
        {
            base.Subscribe();
            if (!IsCompleted)
            {
                EventManager.Instance.Subscribe<BurglarChestOpenedEvent>(OnBurglarChestOpened);
                EventManager.Instance.Subscribe<MineChestOpenedEvent>(OnMineChestOpen);
            }
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            EventManager.Instance.Unsubscribe<BurglarChestOpenedEvent>(OnBurglarChestOpened);
            EventManager.Instance.Unsubscribe<MineChestOpenedEvent>(OnMineChestOpen);
        }


        private void OnMineChestOpen(MineChestOpenedEvent eventData)
        {
            Collect();
        }

        private void OnBurglarChestOpened(BurglarChestOpenedEvent eventData)
        {
            Collect();
        }
    }
}