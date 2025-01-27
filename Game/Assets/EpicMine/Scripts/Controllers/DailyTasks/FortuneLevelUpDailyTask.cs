using BlackTemple.Common;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public sealed class FortuneLevelUpDailyTask : DailyTask
    {
        public FortuneLevelUpDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null) : base(staticTask, dtoTask)
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
            if (eventData.SkillLevel.Type != SkillType.Fortune)
                return;

            Collect();
        }
    }
}