namespace BlackTemple.EpicMine
{
    public struct DailyTaskCompleteEvent
    {
        public DailyTask Task;

        public DailyTaskCompleteEvent(DailyTask dailyTask)
        {
            Task = dailyTask;
        }
    }
}