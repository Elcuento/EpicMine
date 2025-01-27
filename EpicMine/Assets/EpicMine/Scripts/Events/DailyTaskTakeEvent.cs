namespace BlackTemple.EpicMine
{
    public struct DailyTaskTakeEvent
    {
        public DailyTask DailyTask;

        public DailyTaskTakeEvent(DailyTask dailyTask)
        {
            DailyTask = dailyTask;
        }
    }
}