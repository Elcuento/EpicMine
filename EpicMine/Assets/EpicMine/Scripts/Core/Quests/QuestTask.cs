using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class QuestTask
    {
        public CommonDLL.Static.QuestTask StaticTask { get; private set; }

        public Quest Quest { get; private set; }

        public bool IsReady
        {
            get
            {
                if (!_isReady)
                {
                    _isReady = GoalsList.All(x => x.IsCompleted);

                    return _isReady;
                }

                return true;
            }
        }

        public bool IsOpen
        {
            get
            {
                if (!_isOpen)
                {
                    _isOpen = App.Instance.Player.Quests.IsAllGoalsCompleted(StaticTask.RequireGoals)
                              && App.Instance.Player.Quests.IsAllTasksCompleted(StaticTask.RequireTasks);

                    return _isOpen;
                }

               return true;
            }
        }
            

        public bool IsCompleted { get; private set; }

        public List<QuestTaskGoal> GoalsList;

        public bool Initialized;

        private bool _isOpen;

        private bool _isReady;


        public QuestTask(Quest quest, CommonDLL.Static.QuestTask staticTask, CommonDLL.Dto.QuestTask task)
        {
            GoalsList = new List<QuestTaskGoal>();
            StaticTask = staticTask;
            Quest = quest;
            IsCompleted = task.isCompleted;

            for (var index = 0; index < staticTask.Goals.Count; index++)
            {
                var staticTaskGoal = staticTask.Goals[index];
                var goal = App.Instance.StaticData.QuestTaskGoals.Find(x => x.Id == staticTaskGoal);

                if (goal == null)
                {
                    Debug.LogError("Static error on quest foal " + staticTaskGoal);
                    continue;
                }

                if (IsCompleted)
                {
                    GoalsList.Add(new QuestTaskGoal(this, goal, true, OnCompleteGoal));
                }
                else
                {
                    var exist = task.goals.Find(x => x.id == staticTaskGoal);
                    GoalsList.Add(new QuestTaskGoal(this, goal, exist, OnCompleteGoal));
                }
            }
        }

        public QuestTask(Quest quest, CommonDLL.Static.QuestTask staticTask)
        {
            StaticTask = staticTask;
            Quest = quest;
            IsCompleted = false;

            GoalsList = new List<QuestTaskGoal>();

            for (var index = 0; index < StaticTask.Goals.Count; index++)
            {
                var goalId = StaticTask.Goals[index];
                var goal = App.Instance.StaticData.QuestTaskGoals.Find(x => x.Id == goalId);
                GoalsList.Add(new QuestTaskGoal(this, goal, false, OnCompleteGoal));
            }
        }

        private void Subscribe()
        {
          
        }

        private void UnSubscribe()
        {
       
        }

        private void OnCompleteGoal()
        {
            if (IsReady)
            {
                if (StaticTask.IsManualComplete)
                    return;

                if (GoalsList.All(x=>x.StaticGoal.Type == QuestTaskType.CollectCurrency || x.StaticGoal.Type == QuestTaskType.Collect))
                    return;

                Complete();

            }
        }

        public void ReachGoal(QuestTaskType type, string key)
        {
            var goal = GoalsList.Find(x => x.StaticGoal.Type == type && x.StaticGoal.Goal.Key == key);
            goal?.ReachGoal();
        }

        public void Complete()
        {
            if (IsCompleted)
                return;

            if (IsReady)
            {
                for (var index = 0; index < GoalsList.Count; index++)
                {
                    var questTaskGoal = GoalsList[index];
                    questTaskGoal.Check();

                    if (!questTaskGoal.IsCompleted)
                        return;
                }

                for (var index = 0; index < GoalsList.Count; index++)
                {
                    var questTaskGoal = GoalsList[index];
                    questTaskGoal.Complete();
                }

                IsCompleted = true;

                Quest.Save();

                UnSubscribe();

                EventManager.Instance.Publish(new QuestTaskCompleteEvent(this));
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.QuestTaskCompleted);
            }

            App.Instance.Services.AnalyticsService.CompleteTask(StaticTask.Id);
        }

        public void Initialize()
        {
            Initialized = true;

            if (IsCompleted)
                return;

            foreach (var goal in GoalsList)
            {
                goal.Initialize();
            }

            Subscribe();
        }


        public void Speak(CharacterType type)
        {
            var goal = GoalsList.Find(x =>
                x.StaticGoal.Type == QuestTaskType.Speak && x.StaticGoal.Goal.Key == type.ToString());

            if (goal == null)
            {
               App.Instance.Services.LogService.LogError("Not exist goal for dialogue");
                return;
            }

            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(x => x.Id == goal.StaticGoal.Id);

            if (dialogue == null)
            {
                ReachGoal(QuestTaskType.Speak, type.ToString());
            }
            else
            {
                WindowManager.Instance.Show<WindowDialogue>()
                    .Initialize(dialogue,
                () => { ReachGoal(QuestTaskType.Speak, type.ToString()); });
            }
        }

        public void Deactivate()
        {
            UnSubscribe();

            foreach (var questTaskGoal in GoalsList)
            {
              //  if (questTaskGoal.Initialized)
               //     continue;

                questTaskGoal.Deactivate();
            }
        }
    }
}