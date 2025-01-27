using System.Collections.Generic;
using UnityEngine;

namespace CommonDLL.Dto
{
    public class QuestTask
    {
        public string id;
        public bool isCompleted;
        public List<QuestTaskGoal> goals;

        public QuestTask()
        {

        }
    }
}