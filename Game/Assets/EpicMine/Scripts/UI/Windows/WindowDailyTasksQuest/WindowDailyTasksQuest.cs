using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Quest = BlackTemple.EpicMine.Core.Quest;

namespace BlackTemple.EpicMine
{
    public class WindowDailyTasksQuest : WindowBase
    {
        [Header("Tabs")]
        [SerializeField] private Toggle _filterToggleDailyTasks;
        [SerializeField] private Toggle _filterToggleQuests;
        [SerializeField] private TextMeshProUGUI _filterTitleDailyTasks;
        [SerializeField] private TextMeshProUGUI _filterTitleQuests;

        [Space]
        [SerializeField] private Color _activeToggleColor;
        [SerializeField] private Color _unActiveToggleColor;

        [Header("Contents")]
        [SerializeField] private GameObject _dailyTasksContent;
        [SerializeField] private GameObject _questsTasksContent;

        [Header("Daily Tasks")]
        [SerializeField] private WindowDailyTasksQuestTaskTaskItem _taskPrefab;
        [SerializeField] private GameObject _description;
        [SerializeField] private Transform _tasksContainer;

        [Space]
        [SerializeField] private RedDotBaseView _taskRedDot;

        private readonly List<WindowDailyTasksQuestTaskTaskItem> _tasks = new List<WindowDailyTasksQuestTaskTaskItem>();

      
        [Header("Quests")]
        public Action<Quest> OnSelectQuest;

        [Space]
        [SerializeField] private WindowDailyTaskQuestQuestToggleItem _questsLeftPanelTogglePrefab;
        [SerializeField] private ToggleGroup _questsLeftPanelToggleGroup;
        [SerializeField] private ScrollRect _questsLeftPanelScroll;

        [Space]
        [SerializeField] private RedDotBaseView _questsRedDot;

        [Header("Quest/Content")]
        [SerializeField] private WindowDailyTaskQuestQuestDescription _questsDescription;
        [SerializeField] private GameObject _questsEmpty;

        [Space]
        [SerializeField] private WindowDailyTaskQuestQuestTaskClosedItem _questsRightPanelClosedTask;
        [SerializeField] private WindowDailyTaskQuestQuestTaskOpenItem _questsRightPanelOpenTask;
        [SerializeField] private WindowDailyTaskQuestQuestRewardItem _questsRightPanelQuestReward;
        [SerializeField] private WindowDailyTaskQuestQuestGoalsInfoItem WindowDailyTaskQuestQuestGoalsInfoItem;

       [SerializeField] private ScrollRect _questsRightPanelScroll;

        private Quest _lastSelectedQuest;

        private void Start()
        {
            App.Instance.Controllers.RedDotsController.OnQuestsChange += OnQuestsViewedRedDot;      
        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            App.Instance.Controllers.RedDotsController.OnQuestsChange -= OnQuestsViewedRedDot;
        }

        private void OnQuestsViewedRedDot(bool state)
        {
            _questsRedDot.Show(!state ? 1 : 0);
        }

        private void OnQuestCompleted(QuestCompleteEvent data)
        {
            OnToggleCategory(true);
        }

        private void OnTaskCompleted(QuestTaskCompleteEvent data)
        {
            OnToggleCategory(true);
        }

        private void OnGoalCompleted(QuestTaskGoalCompleteEvent eventData)
        {
            OnToggleCategory(true);
        }

        public void OpenQuests()
        {
            _filterToggleQuests.isOn = true;
            OnToggleCategory(true);
        }

        public void OpenLastTab()
        {
            if (PlayerPrefsHelper.LoadDefault(PlayerPrefsType.WindowQuestTaskLastTab, 0) == 0)
                _filterToggleDailyTasks.isOn = true;
            else _filterToggleQuests.isOn = true;

            OnToggleCategory(true);
        }

        public void OpenDaily()
        {
            _filterToggleDailyTasks.isOn = true;
            OnToggleCategory(true);
        }

        public void OnToggleCategory(bool state)
        {
            if (!gameObject.activeSelf)
                return;

            if (!state)
                return;

            _questsLeftPanelScroll.content.ClearChildObjects();
            _questsRightPanelScroll.content.ClearChildObjects(2);

            FilterCategory();

            if (_filterToggleQuests.isOn)
            {
                App.Instance.Controllers.RedDotsController.ViewQuestsWindow();
                _questsRedDot.Show(0);

                _questsDescription.Hide();
                _questsEmpty.SetActive(true);

                var quests = App.Instance.Player.Quests.QuestList.Where(x=> x.Status == QuestStatusType.Started)
                    .ToList();

                for (var i = 0; i < quests.Count; i++)
                {
                    var togglePrefab = Instantiate(_questsLeftPanelTogglePrefab, _questsLeftPanelScroll.content, false);
                    togglePrefab.Initialize(quests[i], _questsLeftPanelToggleGroup, SelectQuest, OnSelectQuest,
                        _lastSelectedQuest == quests[i]);

                    if (i == 0 && _lastSelectedQuest == null)
                    SelectQuest(quests[i]);
                }

                if (_lastSelectedQuest == null && quests.Count > 0)
                {
                    SelectQuest(quests[0]);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(_questsRightPanelScroll.content);
                LayoutRebuilder.ForceRebuildLayoutImmediate(_questsRightPanelScroll.content);

                PlayerPrefsHelper.Save(PlayerPrefsType.WindowQuestTaskLastTab, 1);

            }else if (_filterToggleDailyTasks.isOn)
            {
                PlayerPrefsHelper.Save(PlayerPrefsType.WindowQuestTaskLastTab,0);
                _taskRedDot.Show(0);
                App.Instance.Controllers.RedDotsController.ViewDailyTasks();
            }

        }


        public void ForceSelectQuest(Quest quest)
        {
            if (quest == null)
                return;

            if (!_filterToggleQuests.isOn)
                _filterToggleQuests.isOn = true;

            SelectQuest(quest);
        }

        private void SelectQuest(Quest quest)
        {
            if (quest == null)
                return;

            _lastSelectedQuest = quest;

            _questsRightPanelScroll.content.ClearChildObjects(2);

            _questsEmpty.SetActive(false);
            _questsDescription.Initialize(quest);

            var sortedTasks = quest.TaskList.OrderByDescending(x => x.IsCompleted || x.IsOpen);

            foreach (var questTask in sortedTasks)
            {
                var isOpen = questTask.IsOpen;

                if (questTask.IsCompleted || !isOpen)
                {
                    var questTaskPrefab = Instantiate(_questsRightPanelClosedTask, _questsRightPanelScroll.content, false);
                    questTaskPrefab.Initialize(questTask);
                }
                else
                {
                    var questTaskPrefab = Instantiate(_questsRightPanelOpenTask, _questsRightPanelScroll.content, false);
                    questTaskPrefab.Initialize(questTask);
                }
            }

            Instantiate(_questsRightPanelQuestReward, _questsRightPanelScroll.content, false)
                .Initialize(quest);

            Instantiate(WindowDailyTaskQuestQuestGoalsInfoItem, _questsRightPanelScroll.content, false)
                .Initialize(quest);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_questsRightPanelScroll.content);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_questsRightPanelScroll.content);

        }

        public void FilterCategory()
        {
            _filterTitleDailyTasks.color = _filterToggleDailyTasks.isOn ? _activeToggleColor : _unActiveToggleColor;
            _filterTitleQuests.color = _filterToggleQuests.isOn ? _activeToggleColor : _unActiveToggleColor;

            _dailyTasksContent.gameObject.SetActive(_filterToggleDailyTasks.isOn);
            _questsTasksContent.gameObject.SetActive(_filterToggleQuests.isOn);

        }

        
        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            Clear();

            App.Instance.Controllers.DailyTasksController.Tasks.Sort(SortByCompletedAndImportant);

            foreach (var dailyTask in App.Instance.Controllers.DailyTasksController.Tasks)
            {
                if (dailyTask.IsCompleted && dailyTask.IsRewardTaken)
                    continue;

                var task = Instantiate(_taskPrefab, _tasksContainer, false);
                task.Initialize(dailyTask);

                _tasks.Add(task);
            }

            OnTasksUpdate();

            ShowRedDots();

            EventManager.Instance.Subscribe<DailyTaskTakeEvent>(OnTaskTaken);
            EventManager.Instance.Subscribe<QuestTaskCompleteEvent>(OnTaskCompleted);
            EventManager.Instance.Subscribe<QuestTaskGoalCompleteEvent>(OnGoalCompleted);
            EventManager.Instance.Subscribe<QuestCompleteEvent>(OnQuestCompleted);

        }

        public override void OnClose()
        {
            base.OnClose();

            App.Instance.Controllers.RedDotsController.ViewQuestsWindow();

            Clear();
        }


        private void OnTaskTaken(DailyTaskTakeEvent eventData)
        {
            var task = _tasks.FirstOrDefault(t => t.Task.StaticTask.Id == eventData.DailyTask.StaticTask.Id);
            if (task != null)
            {
                _tasks.Remove(task);
                Destroy(task.gameObject);
                OnTasksUpdate();
            }
        }

        private void OnTasksUpdate()
        {
            _description.SetActive(_tasks.Count <= 0);
        }

        private void Clear()
        {
            foreach (var windowDailyTasksTask in _tasks)
                Destroy(windowDailyTasksTask.gameObject);

            _tasks.Clear();

            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<DailyTaskTakeEvent>(OnTaskTaken);
            EventManager.Instance.Unsubscribe<QuestTaskCompleteEvent>(OnTaskCompleted);
            EventManager.Instance.Unsubscribe<QuestTaskGoalCompleteEvent>(OnGoalCompleted);
            EventManager.Instance.Unsubscribe<QuestCompleteEvent>(OnQuestCompleted);
        }

        private int SortByCompletedAndImportant(DailyTask x, DailyTask y)
        {
            if (x.IsCompleted)
            {
                if (!y.IsCompleted)
                    return -1;

                if (x.StaticTask.Important)
                    return y.StaticTask.Important ? 0 : -1;

                return y.StaticTask.Important ? 1 : 0;
            }

            if (y.IsCompleted)
                return 1;

            if (x.StaticTask.Important)
                return y.StaticTask.Important ? 0 : -1;

            return y.StaticTask.Important ? 1 : 0;
        }

        private void ShowRedDots()
        {
            var taskCount = 0;

            foreach (var windowDailyTasksTask in _tasks)
            {
                windowDailyTasksTask.HideRedDot();

                if (windowDailyTasksTask.Task.IsCompleted)
                    continue;

                if (!App.Instance.Controllers.RedDotsController.ViewedDailyTasks.Contains(windowDailyTasksTask.Task
                    .StaticTask.Id))
                {
                    windowDailyTasksTask.ShowRedDot();
                    taskCount++;
                }
            }

            _questsRedDot.Show(App.Instance.Controllers.RedDotsController.IsQuestsChangeShowed ? 0 : 1);
            _taskRedDot.Show(taskCount);
        }
    }
}