using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class QuestTask
    {
        public string Id { get; }

        public List<string> Goals { get; }

        public string EndDescription { get; }

        public List<string> RequireTasks { get; }

        public List<string> RequireGoals { get; }

        public bool IsManualComplete;

        public QuestTask(string id, string goals, string requireTask, string requireGoals, string endDescription, bool? isManualComplete)
        {
            Id = id;
            Goals = Extensions.SplitToList<string>(goals, "#");
            RequireGoals = Extensions.SplitToList<string>(requireGoals, "#");
            RequireTasks = Extensions.SplitToList<string>(requireTask, "#");
            EndDescription = endDescription;
            IsManualComplete = isManualComplete ?? false;
        }
    }
}