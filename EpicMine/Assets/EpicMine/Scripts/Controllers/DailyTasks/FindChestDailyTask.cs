using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public sealed class FindChestDailyTask : DailyTask
    {
        public FindChestDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null) : base(staticTask, dtoTask)
        {
            Subscribe();
        }


        public override void Subscribe()
        {
            base.Subscribe();
            if (!IsCompleted)
                EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
        }


        private void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            if (eventData.Section is MineSceneChestSection)
                Collect();
        }
    }
}