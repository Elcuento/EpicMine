using System.Collections.Generic;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class Quest
    {
        public string id;
        public List<QuestTask> tasks;
        public QuestStatusType status;
        public bool isTracking;

        public Quest(string id, List<QuestTask> tasks, QuestStatusType status, bool isTracking)
        {
            this.id = id;
            this.tasks = tasks;
            this.status = status;
            this.isTracking = isTracking;
        }
    }
}