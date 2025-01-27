using BlackTemple.Common;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public sealed class CritLevelUpDailyTask : DailyTask
    {
        public CritLevelUpDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null) : base(staticTask, dtoTask)
        {
            Subscribe();
        }


        public override void Subscribe()
        {
            base.Subscribe();

            if (!IsCompleted)
                EventManager.Instance.Subscribe<SkillLevelChangeEvent>(OnCharacteristicLevelChange);
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            EventManager.Instance.Unsubscribe<SkillLevelChangeEvent>(OnCharacteristicLevelChange);
        }


        private void OnCharacteristicLevelChange(SkillLevelChangeEvent eventData)
        {
            if (eventData.SkillLevel.Type != SkillType.Crit)
                return;

            Collect();
        }
    }
}