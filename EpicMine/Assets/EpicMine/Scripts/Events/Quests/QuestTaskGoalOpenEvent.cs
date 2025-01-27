using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct QuestTaskGoalOpenEvent
    {
        public QuestTask Task;

        public QuestTaskGoalOpenEvent(QuestTask task)
        {
            Task = task;
        }
    }
}