using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct QuestTaskGoalCompleteEvent
    {
        public QuestTaskGoal Goal;

        public QuestTaskGoalCompleteEvent(QuestTaskGoal goal)
        {
            Goal = goal;
        }
    }
}