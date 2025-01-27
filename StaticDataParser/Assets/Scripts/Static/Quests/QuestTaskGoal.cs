using System.Collections.Generic;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class QuestTaskGoal
    {
        public string Id { get; }

        public QuestTaskType Type { get; }

        public KeyValuePair<string, int> Goal { get; }

        public Dictionary<QuestRequirementsType, string> PlacementRequire;

        public QuestTaskGoal(string id, QuestTaskType type, string goal, string placementRequire)
        {

            Id = id;
            Type = type;
            PlacementRequire =
                Extensions.GetDictionaryBySplitKeyValuePair<QuestRequirementsType, string>(placementRequire, '#');

            Goal = Extensions.GetStringSplitKeyPair<string, int>(goal);
        }
    }
}
