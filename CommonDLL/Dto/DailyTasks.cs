using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class DailyTasks
    {
        public List<DailyTask> TodayTaken;

        public DailyTasks()
        {
            TodayTaken = new List<DailyTask>();
        }
    }
}