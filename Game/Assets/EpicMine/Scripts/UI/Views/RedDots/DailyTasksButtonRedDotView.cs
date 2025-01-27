using System.Collections.Generic;

namespace BlackTemple.EpicMine
{
    public class DailyTasksButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            App.Instance.Controllers.RedDotsController.OnDailyTasksChange += OnDailyTasksChange;
            App.Instance.Controllers.RedDotsController.OnQuestsChange += OnQuestsChanged;
        }

        private void Start()
        {
            CalculateDot();
        }

        private void OnDestroy()
        {
            if (App.Instance == null)
                return;

            App.Instance.Controllers.RedDotsController.OnDailyTasksChange -= OnDailyTasksChange;
            App.Instance.Controllers.RedDotsController.OnQuestsChange -= OnQuestsChanged;
        }

        private void OnDailyTasksChange(List<string> viewed)
        {
            CalculateDot();
        }

        private void OnQuestsChanged(bool viewed)
        {
            CalculateDot();
        }

        private void CalculateDot()
        {
            var tasks = new List<string>();

            foreach (var dailyTask in App.Instance.Controllers.DailyTasksController.Tasks)
            {
                if (dailyTask.IsCompleted && dailyTask.IsRewardTaken)
                    continue;

                if (!dailyTask.IsCompleted && App.Instance.Controllers.RedDotsController.ViewedDailyTasks.Contains(dailyTask.StaticTask.Id))
                    continue;

                tasks.Add(dailyTask.StaticTask.Id);
            }

            Show(tasks.Count + (App.Instance.Controllers.RedDotsController.IsQuestsChangeShowed ? 0 : 1));
        }
    }
}