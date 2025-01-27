using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct QuestTaskGoalChangeEvent
    {
        public QuestTaskGoal Goal;

        public QuestTaskGoalChangeEvent(QuestTaskGoal goal)
        {
            Goal = goal;
        }
    }
}