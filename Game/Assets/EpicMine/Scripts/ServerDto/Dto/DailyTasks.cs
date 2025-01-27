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
        public DailyTasks(BlackTemple.EpicMine.Core.DailyTasks data)
        {
            TodayTaken = new List<DailyTask>();

            foreach (var a in data.TodayTaken)           
            {
                TodayTaken.Add(new DailyTask()
                {
                    Id = a.Id,
                    TakenTime = a.Taken
                });
            }
        }
    }
}