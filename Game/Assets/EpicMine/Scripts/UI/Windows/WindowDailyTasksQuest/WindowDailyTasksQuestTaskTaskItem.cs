using System;
using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowDailyTasksQuestTaskTaskItem : MonoBehaviour
    {
        [SerializeField] private GameObject _completed;
        [SerializeField] private Slider _progress;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private Button _takeRewardButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private GameObject _redDot;

        public DailyTask Task { get; private set; }


        public void Initialize(DailyTask task)
        {
            Task = task;

            _completed.SetActive(Task.IsCompleted);
            _takeRewardButton.gameObject.SetActive(Task.IsCompleted);

            var title = LocalizationHelper.GetLocale(Task.StaticTask.Id);
            if (Task.StaticTask.Important)
            {
                var importantLocale = LocalizationHelper.GetLocale("window_daily_task_quest_task_important");
                title += $" {importantLocale}";
            }
            _titleText.text = title;

            var descriptionLocale = LocalizationHelper.GetLocale("daily_task_" + Task.StaticTask.Type.ToString().ToLower());

            switch (task.StaticTask.Type)
            {
                case DailyTaskType.ActualOreMining:
                    var actualOreMiningTask = (ActualOreMiningDailyTask) task;
                    var actualOreLocale = LocalizationHelper.GetLocale(actualOreMiningTask.OreStaticId);
                    _descriptionText.text = string.Format(descriptionLocale, Task.StaticTask.RequiredAmount, actualOreLocale);
                    break;
                case DailyTaskType.ObsoleteOreMining:
                    var obsoleteOreMiningTask = (ObsoleteOreMiningDailyTask)task;
                    var obsoleteOreLocale = LocalizationHelper.GetLocale(obsoleteOreMiningTask.OreStaticId);
                    _descriptionText.text = string.Format(descriptionLocale, Task.StaticTask.RequiredAmount, obsoleteOreLocale);
                    break;
                case DailyTaskType.DamageLevelUp:
                case DailyTaskType.CritLevelUp:
                case DailyTaskType.FortuneLevelUp:
                case DailyTaskType.FindChest:
                case DailyTaskType.BreakChest:
                    _descriptionText.text = string.Format(descriptionLocale, Task.StaticTask.RequiredAmount);
                    break;
                case DailyTaskType.UnlockMine:
                case DailyTaskType.UnlockTier:
                case DailyTaskType.PerfectMineComplete:
                    _descriptionText.text = descriptionLocale;
                    break;
                case DailyTaskType.CraftActualIngot:
                    var craftIngotTask = (CraftActualIngotDailyTask)task;
                    var ingotTask = LocalizationHelper.GetLocale(craftIngotTask.IngotStaticId);
                    _descriptionText.text = string.Format(descriptionLocale, Task.StaticTask.RequiredAmount, ingotTask);
                    break;
                case DailyTaskType.TradeAffairs:
                    var tradeIngotTask = (TradeAffairsDailyTask)task;
                    var tradeIngotLocalization = LocalizationHelper.GetLocale(tradeIngotTask.IngotStaticId);
                    _descriptionText.text = string.Format(descriptionLocale, Task.StaticTask.RequiredAmount, tradeIngotLocalization);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _rewardText.text = Task.StaticTask.RewardAmount.ToString();

            if (Task.StaticTask.RequiredAmount > 1)
            {
                _progress.gameObject.SetActive(true);
                _progress.value = (float)Task.CollectedAmount / Task.StaticTask.RequiredAmount;
                _progressText.text = $"{ Mathf.Clamp(Task.CollectedAmount, 0, Task.StaticTask.RequiredAmount) }/{ Task.StaticTask.RequiredAmount }";
            }
        }


        public void TakeRewardButtonClick()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            Task.TakeReward();
        }

        public void ShowRedDot()
        {
            _redDot.SetActive(true);
        }

        public void HideRedDot()
        {
            _redDot.SetActive(false);
        }
    }
}