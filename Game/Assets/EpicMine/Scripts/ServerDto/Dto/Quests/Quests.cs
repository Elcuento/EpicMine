using System.Collections.Generic;
using UnityEngine;

namespace CommonDLL.Dto
{
    public class Quests
    {
        public List<Quest> QuestList;

        public Quests()
        {
            QuestList = new List<Quest>();
        }

        public Quests(BlackTemple.EpicMine.Core.Quests data)
        {
            QuestList = new List<Quest>();

            foreach (var quest in data.QuestList)
            {
                var questTasks = new List<QuestTask>();
  
                foreach (var task in quest.TaskList)
                {
                    var questTask = new QuestTask();

                    questTask.id = task.StaticTask.Id;
                    questTask.isCompleted = task.IsCompleted;
                    questTask.goals = new List<QuestTaskGoal>();

                    foreach (var questTaskGoal in task.GoalsList)
                    {
                        questTask.goals.Add(new QuestTaskGoal()
                        {
                            id = questTaskGoal.StaticGoal.Id,
                            isCompleted = questTaskGoal.IsCompleted,
                            progress = questTaskGoal.Progress,
                            startTime = questTaskGoal.StartTime
                        });
                    }

                    questTasks.Add(questTask);
                }

                QuestList.Add(new Quest(quest.StaticQuest.Id, questTasks, quest.Status, quest.IsTracking));
            }


        }
    }
}