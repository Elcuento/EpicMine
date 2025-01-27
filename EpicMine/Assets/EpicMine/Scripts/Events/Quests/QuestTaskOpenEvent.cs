using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct QuestTaskOpenEvent
    {
        public QuestTask Task;

        public QuestTaskOpenEvent(QuestTask task)
        {
            Task = task;
        }
    }
}