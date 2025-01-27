using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class QuestTask
    {
        public string id;
        public bool isCompleted;
        public List<QuestTaskGoal> goals;
    }
}