using System.Linq;
using System.Linq.Expressions;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class ShowDailyTasksTutorialStep : TutorialStepBase
    {
        public ShowDailyTasksTutorialStep(bool isComplete) : base(TutorialStepIds.ShowDailyTasks, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowCharacteristics))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Village)
                return;

            var villageSceneController = Object.FindObjectOfType<VillageSceneController>();
            if (villageSceneController != null)
                villageSceneController.ScrollToTasks();

            SetReady();
        }

        public override void Clear()
        {
            base.Clear();
            Unsubscribe();
        }


        protected override void OnReady()
        {
            if (!Check())
            {
                SetComplete();
                return;
            }
            
            var window = WindowManager.Instance.Show<WindowDialogue>();
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_show_daily_tasks");
            window.Initialize(dialogue, () =>
                {
                    WindowManager.Instance.Show<WindowDailyTasksFirstPartTutorialStepAssistant>();
                });
        }

        private bool Check()
        {
            if (App.Instance.Player.Quests.QuestList.Find(x => x.Status == QuestStatusType.Completed) != null
                && App.Instance.Controllers.DailyTasksController.Tasks.Find(x => x.IsCompleted) == null)
            {
                return false;
            }

            return true;
        }

        protected override void OnComplete()
        {
            Unsubscribe();

            var villageSceneController = Object.FindObjectOfType<VillageSceneController>();
            if (villageSceneController != null)
                villageSceneController.ScrollToMine();
        }


        private void OnSceneChange(string from, string to)
        {
            if (to == ScenesNames.Village)
                CheckReady();
        }

        private void OnTutorialStepComplete(TutorialStepCompleteEvent eventData)
        {
            if (eventData.Step.Id == TutorialStepIds.ShowCharacteristics)
                CheckReady();
        }

        private void OnWindowClose(WindowCloseEvent eventData)
        {
            if (eventData.Window is WindowDailyTasksQuest)
                SetComplete();
        }



        private void Subscribe()
        {
            SceneManager.Instance.OnSceneChange += OnSceneChange;
            EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnTutorialStepComplete);
            EventManager.Instance.Subscribe<WindowCloseEvent>(OnWindowClose);
        }

        private void Unsubscribe()
        {
            if (SceneManager.Instance != null)
                SceneManager.Instance.OnSceneChange -= OnSceneChange;

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnTutorialStepComplete);
                EventManager.Instance.Unsubscribe<WindowCloseEvent>(OnWindowClose);
            }
        }
    }
}