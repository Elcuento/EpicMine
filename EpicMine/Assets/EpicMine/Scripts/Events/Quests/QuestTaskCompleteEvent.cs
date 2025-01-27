using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct QuestTaskCompleteEvent
    {
        public QuestTask Task;

        public QuestTaskCompleteEvent(QuestTask task)
        {
            Task = task;
        }
    }
}