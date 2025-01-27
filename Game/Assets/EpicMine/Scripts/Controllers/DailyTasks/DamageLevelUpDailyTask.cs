using BlackTemple.Common;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public sealed class DamageLevelUpDailyTask : DailyTask
    {
        public DamageLevelUpDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null) : base(staticTask, dtoTask)
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
            if (eventData.SkillLevel.Type != SkillType.Damage)
                return;

            Collect();
        }
    }
}