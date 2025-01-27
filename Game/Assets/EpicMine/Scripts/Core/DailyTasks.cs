using System;
using System.Collections.Generic;

namespace BlackTemple.EpicMine.Core
{
    public class DailyTasks
    {
        public class DailyTask
        {
            public string Id;
            public DateTime Taken;
        }
        public List<DailyTask> TodayTaken { get; }

        public DailyTasks(CommonDLL.Dto.DailyTasks dailyTasksResponse)
        {
            TodayTaken = new List<DailyTask>();

            if (dailyTasksResponse != null && dailyTasksResponse.TodayTaken.Count > 0)
            {
                var today = DateTime.UtcNow.Date;
                foreach (var gameDataDailyTaskResponse in dailyTasksResponse.TodayTaken)
                {
                    var takenTime = gameDataDailyTaskResponse.TakenTime;//TimeManager.Instance.FromUnixToDateTime(gameDataDailyTaskResponse.TakenTime);
                    if (takenTime < today)
                        continue;

                    TodayTaken.Add(new DailyTask()
                    {
                        Id = gameDataDailyTaskResponse.Id,
                        Taken = takenTime
                    });
                }
            }
        }
    }
}