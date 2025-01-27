using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class Quests
    {
        public const int QuestEqualLimit = 1;

        public List<Quest> QuestList { get; private set; }


     

        public Quests(CommonDLL.Dto.Quests data)
        {
            QuestList = new List<Quest>();

            foreach (var staticQuest in App.Instance.StaticData.Quests)
            {
                var existQuest = data.QuestList.Find(x=>x.id == staticQuest.Id);

                var inProgress = (existQuest != null && existQuest.tasks != null && existQuest.tasks.Count > 0 &&
                                  !string.IsNullOrEmpty(existQuest.id));

                if (!inProgress && staticQuest.Type == QuestType.Event)
                    continue;

                QuestList.Add(inProgress ? new Quest(staticQuest, existQuest) : new Quest(staticQuest));
            }

            //   Debug.Log("Total quests count " + QuestList.Count);

            EventManager.Instance.Subscribe<QuestTaskCompleteEvent>(OnTaskCompleted);
            EventManager.Instance.Subscribe<QuestTaskGoalCompleteEvent>(OnGoalCompleted);
            EventManager.Instance.Subscribe<QuestCompleteEvent>(OnQuestCompleted);
            EventManager.Instance.Subscribe<QuestActivateEvent>(OnQuestActivated);
            EventManager.Instance.Subscribe<QuestStartEvent>(OnQuestStart);
            EventManager.Instance.Subscribe<QuestTaskGoalChangeEvent>(OnQuestTaskGoalChange);

        }

        private void OnQuestTaskGoalChange(QuestTaskGoalChangeEvent eventData)
        {
            if (eventData.Goal.StaticGoal.Type == QuestTaskType.TimeLeft)
                return;

            WindowManager.Instance.Show<WindowQuestTracking>()
                .Initialize(eventData.Goal);
        }

        private void OnTaskCompleted(QuestTaskCompleteEvent data)
        {
          //  Debug.Log("Task completed " + data.Task.StaticTask.Id);
            Initialize();
        }

        private void OnGoalCompleted(QuestTaskGoalCompleteEvent data)
        {
          // Debug.Log("Goal completed " + data.Goal.StaticGoal.Id);
            Initialize();
        }

        private void OnQuestStart(QuestStartEvent data)
        {
         //   Debug.Log("Quest " + data.Quest.Status +":" + data.Quest.StaticQuest.Id);
            Initialize();
        }

        private void OnQuestActivated(QuestActivateEvent data)
        {
         //   Debug.Log("Quest " + data.Quest.Status + ":" + data.Quest.StaticQuest.Id);
            Initialize();
        }

        private void OnQuestCompleted(QuestCompleteEvent data)
        {
       //     Debug.Log("Quest " + data.Quest.Status + ":" + data.Quest.StaticQuest.Id);
            Initialize();
        }

        public Quest IsExistStartQuest (CharacterType character)
        {
            for (var index = 0; index < QuestList.Count; index++)
            {
                var quest = QuestList[index];
                if (quest.Status != QuestStatusType.Activated ||
                    quest.StaticQuest.StartTriggerType != QuestTriggerType.Speak)
                    continue;

                if (quest.StaticQuest.StartTrigger.Key == QuestTriggerExecuter.Character &&
                    quest.StaticQuest.StartTrigger.Value == character.ToString())
                    return quest;
            }

            return null;
        }

        public QuestTask IsExistInSpeakQuest (CharacterType character)
        {
            for (var index1 = 0; index1 < QuestList.Count; index1++)
            {
                var quest = QuestList[index1];
                if (quest.Status != QuestStatusType.Started || quest.Status == QuestStatusType.Completed)
                    continue;

                for (var i = 0; i < quest.TaskList.Count; i++)
                {
                    var questTask = quest.TaskList[i];
                    if (!questTask.IsOpen || questTask.IsCompleted)
                        continue;

                    for (var index = 0; index < questTask.GoalsList.Count; index++)
                    {
                        var goal = questTask.GoalsList[index];
                        if (goal.IsCompleted)
                            continue;

                        if (goal.StaticGoal.Type == QuestTaskType.Speak &&
                            goal.StaticGoal.Goal.Key == character.ToString())
                        {
                            return questTask;
                        }
                    }
                }
            }

            return null;
        }

        public bool IsAllTasksCompleted(List<string> tasks)
        {
            if (tasks.Count <= 0)
                return true;

            for (var index = 0; index < QuestList.Count; index++)
            {
                var quest = QuestList[index];
                if (quest.Status != QuestStatusType.Started || quest.Status == QuestStatusType.Completed)
                    continue;

                for (var i = 0; i < quest.TaskList.Count; i++)
                {
                    var questTask = quest.TaskList[i];
                    if (tasks.Contains(questTask.StaticTask.Id) && !questTask.IsCompleted)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsAllGoalsCompleted(List<string> goals)
        {
            if (goals.Count <= 0)
                return true;

            for (var index = 0; index < QuestList.Count; index++)
            {
                var quest = QuestList[index];
                for (var i = 0; i < quest.TaskList.Count; i++)
                {
                    var questTask = quest.TaskList[i];          
                    for (var index1 = 0; index1 < questTask.GoalsList.Count; index1++)
                    {
                        var goal = questTask.GoalsList[index1];
                        if (!goals.Contains(goal.StaticGoal.Id))
                            continue;

                        if (!goal.IsCompleted)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        private List<Quest> Filter()
        {
            var qList = new List<Quest>();
            var questEqualDictionary = new Dictionary<string, int>();
            var filter = "";


            for (var index = 0; index < QuestList.Count; index++)
            {
                var quest = QuestList[index];
                if (quest.Status == QuestStatusType.Completed)
                    continue;

                if (quest.Status == QuestStatusType.Started || quest.Status == QuestStatusType.Activated)
                {
                    qList.Add(quest);
                    filter = $"{quest.StaticQuest.Owner}{quest.StaticQuest.Filter}";

                    if (quest.StaticQuest.Type != QuestType.Event && quest.StaticQuest.Type != QuestType.Epic)
                    {
                        if (questEqualDictionary.ContainsKey(filter))
                            questEqualDictionary[filter]++;
                        else questEqualDictionary.Add(filter, 1);
                    }
                }
                else
                {
                    var hasFeature = false; // TODO FOR PRESTIGE

                    foreach (var staticQuestRewardFeature in quest.StaticQuest.RewardFeatures)
                    {
                        if (App.Instance.Player.Features.Exist((FeaturesType)staticQuestRewardFeature))
                        {
                            hasFeature = true;
                            break;
                        }
                    }

                    if (hasFeature)
                        continue;

                    filter = $"{quest.StaticQuest.Owner}{quest.StaticQuest.Filter}";
                    var simple = quest.StaticQuest.Type != QuestType.Event && quest.StaticQuest.Type != QuestType.Epic
                                                                            && quest.StaticQuest.Type != QuestType.Rare;

                    if (simple)
                    {
                        if (questEqualDictionary.ContainsKey(filter))
                        {
                            if (questEqualDictionary[filter] < QuestEqualLimit)
                            {
                                questEqualDictionary[filter]++;
                            }
                            else continue;
                        }
                        else questEqualDictionary.Add(filter, 1);
                    }

                    qList.Add(quest);

                }
            }

            return qList;
        }

        public void Initialize()
        {
            for (var index = 0; index < QuestList.Count; index++)
            {
                var quest = QuestList[index];
                quest.Deactivate();
            }

            var qList = Filter();

            for (var index = 0; index < qList.Count; index++)
            {
                var i = qList[index];
                i.Initialize();
            }

            for (var index = 0; index < qList.Count; index++)
            {
                var i = qList[index];
                i.AfterInitializeCheck();

            }

            App.Instance.Services.LogService.Log("Active Quests " + qList.Count);

        }

        public void Add(CommonDLL.Static.Quest staticQuest)
        {
            if (staticQuest == null)
                return;

            var existQuest = QuestList.Find(x => x.StaticQuest.Id == staticQuest.Id);

            if (existQuest != null)
            {
                Remove(existQuest);
            }

            var quest = new Quest(staticQuest);

             QuestList.Add(quest);

            quest.Initialize();

         //   quest.SetActivate();
        }

        public void Remove(Quest quest)
        {
            if (quest == null)
                return;

            quest.Remove();

            QuestList.Remove(quest);
        }
    }
}