using System.Collections.Generic;

namespace CommonDLL.Static
{
    public class QuestTaskGoal
    {
        public string Id ;

        public QuestTaskType Type ;

        public KeyValuePair<string, int> Goal ;

        public Dictionary<QuestRequirementsType, string> PlacementRequire;

    }
}