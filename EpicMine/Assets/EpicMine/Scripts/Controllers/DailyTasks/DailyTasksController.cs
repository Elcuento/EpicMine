using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class DailyTasksController
    {
        public List<DailyTask> Tasks { get; } = new List<DailyTask>();

        private const string FileName = "dailyTasksData";

        private readonly IStorageService _storageService = new JsonDiskStorageService();


        public DailyTasksController()
        {

            if (_storageService.IsDataExists(FileName))
            {
                var data = _storageService.Load<List<Dto.DailyTask>>(FileName);

                if (data != null && data.Count > 0)
                {
                    var today = DateTime.UtcNow.Date;

                    foreach (var dtoTask in data)
                    {
                        if (Tasks.Count + App.Instance.Player.DailyTasks.TodayTaken.Count >= App.Instance.StaticData.Configs.DailyTasks.MaxCount)
                            break;

                        if (dtoTask.IsRewardTaken && dtoTask.CreationDate < today)
                            continue;

                        if (App.Instance.Player.DailyTasks.TodayTaken.Find(x=> dtoTask.Id == x.Id) != null)
                            continue;

                        var staticTask = App.Instance.StaticData.DailyTasks.FirstOrDefault(t => t.Id == dtoTask.Id);
                        AddTask(staticTask, dtoTask);
                    }
                }

                FillTasks();
            }
            else
                CreateTasksForNewPlayer();
        }

        public void Remove(string taskId)
        {
            var taskStatic = App.Instance.StaticData.DailyTasks.Find(x => x.Id == taskId);
            var task = Tasks.Find(x => x.StaticTask == taskStatic);
            if (task != null)
            {
                Tasks.Remove(task);
                Save();
            }
     
        }

        public void Save()
        {
            var data = new List<Dto.DailyTask>();

            foreach (var dailyTask in Tasks)
            {
                Dto.DailyTask dtoTask;

                switch (dailyTask.StaticTask.Type)
                {
                    case DailyTaskType.ActualOreMining:
                        var actualOreMiningTask = (ActualOreMiningDailyTask)dailyTask;
                        dtoTask = new ItemDailyTask(
                            dailyTask.StaticTask.Id,
                            dailyTask.CreationDate,
                            dailyTask.CollectedAmount,
                            dailyTask.IsRewardTaken,
                            actualOreMiningTask.OreStaticId);
                        break;

                    case DailyTaskType.ObsoleteOreMining:
                        var obsoleteOreMiningTask = (ObsoleteOreMiningDailyTask)dailyTask;
                        dtoTask = new ItemDailyTask(
                            dailyTask.StaticTask.Id,
                            dailyTask.CreationDate,
                            dailyTask.CollectedAmount,
                            dailyTask.IsRewardTaken,
                            obsoleteOreMiningTask.OreStaticId);
                        break;

                    case DailyTaskType.CraftActualIngot:
                        var craftIngotTask = (CraftActualIngotDailyTask)dailyTask;
                        dtoTask = new ItemDailyTask(
                            dailyTask.StaticTask.Id,
                            dailyTask.CreationDate,
                            dailyTask.CollectedAmount,
                            dailyTask.IsRewardTaken,
                            craftIngotTask.IngotStaticId);
                        break;

                    case DailyTaskType.TradeAffairs:
                        var tradeIngotTask = (TradeAffairsDailyTask)dailyTask;
                        dtoTask = new ItemDailyTask(
                            dailyTask.StaticTask.Id,
                            dailyTask.CreationDate,
                            dailyTask.CollectedAmount,
                            dailyTask.IsRewardTaken,
                            tradeIngotTask.IngotStaticId);
                        break;

                    default:
                        dtoTask = new Dto.DailyTask(
                            dailyTask.StaticTask.Id,
                            dailyTask.CreationDate,
                            dailyTask.CollectedAmount,
                            dailyTask.IsRewardTaken);
                        break;
                }

                data.Add(dtoTask);
            }

            _storageService.Save(FileName, data);
        }

        public void Clear()
        {
            _storageService.Remove(FileName);
        }

        public void FillTasks()
        {
            var needTasksCount = App.Instance.StaticData.Configs.DailyTasks.MaxCount - App.Instance.Player.DailyTasks.TodayTaken.Count - Tasks.Count;
            if (needTasksCount <= 0)
                return;

            for (var i = 0; i < needTasksCount; i++)
                CreateRandomTask();
        }


        private void CreateTasksForNewPlayer()
        {
            var unlockTierTask = App.Instance.StaticData.DailyTasks.FirstOrDefault(t => t.Type == DailyTaskType.UnlockTier);
            var upgradeDamageTask = App.Instance.StaticData.DailyTasks.FirstOrDefault(t => t.Type == DailyTaskType.DamageLevelUp);
            var tradeAffairsTask = App.Instance.StaticData.DailyTasks.FirstOrDefault(t => t.Type == DailyTaskType.TradeAffairs);

            var existTasksCount = Tasks.Count + App.Instance.Player.DailyTasks.TodayTaken.Count;
            var tasksToCreateLeft = App.Instance.StaticData.Configs.DailyTasks.MaxCount - existTasksCount;

            if (unlockTierTask != null && tasksToCreateLeft > 0 && App.Instance.Player.DailyTasks.TodayTaken.Find(x=>x.Id == unlockTierTask.Id) == null)
                AddTask(unlockTierTask);

            if (upgradeDamageTask != null && tasksToCreateLeft > 1 && App.Instance.Player.DailyTasks.TodayTaken.Find(x => x.Id == upgradeDamageTask.Id) == null)
                AddTask(upgradeDamageTask);

            if (tradeAffairsTask != null && tasksToCreateLeft > 2 && App.Instance.Player.DailyTasks.TodayTaken.Find(x => x.Id == tradeAffairsTask.Id) == null)
                AddTask(tradeAffairsTask);
        }

        private void CreateRandomTask()
        {
            var categories = new List<int>();

            var skillCap = App.Instance.Player.Skills.Crit.IsLast &&
                           App.Instance.Player.Skills.Damage.IsLast &&
                           App.Instance.Player.Skills.Fortune.IsLast;

            foreach (var staticTask in App.Instance.StaticData.DailyTasks)
            {
                if (Tasks.Any(t => t.StaticTask.Category == staticTask.Category))
                    continue;

                if (App.Instance.Player.DailyTasks.TodayTaken.Find(x => staticTask.Id == x.Id) != null)
                    continue;

                if (categories.Contains(staticTask.Category))
                    continue;

                if ((staticTask.Type == DailyTaskType.CritLevelUp ||
                     staticTask.Type == DailyTaskType.DamageLevelUp ||
                     staticTask.Type == DailyTaskType.FortuneLevelUp) && skillCap)
                    continue;

                categories.Add(staticTask.Category);
            }

            if (categories.Count == 0)
                return;

            var randomCategoryIndex = Random.Range(0, categories.Count);
            var category = categories[randomCategoryIndex];

            var tasks = App.Instance.StaticData.DailyTasks.Where(t => t.Category == category).ToList();

            for (var i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].Type == DailyTaskType.CritLevelUp && App.Instance.Player.Skills.Crit.IsLast)
                {
                    tasks.Remove(tasks[i]);
                    i--;
                    continue;
                }

                if (tasks[i].Type == DailyTaskType.DamageLevelUp && App.Instance.Player.Skills.Damage.IsLast)
                {
                    tasks.Remove(tasks[i]);
                    i--;
                    continue;
                }

                if (tasks[i].Type == DailyTaskType.FortuneLevelUp && App.Instance.Player.Skills.Fortune.IsLast)
                {
                    tasks.Remove(tasks[i]);
                    i--;
                }
            }

            if (tasks.Count == 0) return;

            var randomTaskIndex = Random.Range(0, tasks.Count);
            var randomTask = tasks[randomTaskIndex];
            AddTask(randomTask);
        }

        private void AddTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null)
        {
            DailyTask newTask;

            switch (staticTask.Type)
            {
                case DailyTaskType.ActualOreMining:
                    newTask = new ActualOreMiningDailyTask(staticTask, (Dto.ItemDailyTask)dtoTask);
                    break;
                case DailyTaskType.ObsoleteOreMining:
                    newTask = new ObsoleteOreMiningDailyTask(staticTask, (Dto.ItemDailyTask)dtoTask);
                    break;
                case DailyTaskType.DamageLevelUp:
                    newTask = new DamageLevelUpDailyTask(staticTask, dtoTask);
                    break;
                case DailyTaskType.CritLevelUp:
                    newTask = new CritLevelUpDailyTask(staticTask, dtoTask);
                    break;
                case DailyTaskType.FortuneLevelUp:
                    newTask = new FortuneLevelUpDailyTask(staticTask, dtoTask);
                    break;
                case DailyTaskType.UnlockMine:
                    newTask = new UnlockNewMineDailyTask(staticTask, dtoTask);
                    break;
                case DailyTaskType.UnlockTier:
                    newTask = new UnlockNewTierDailyTask(staticTask, dtoTask);
                    break;
                case DailyTaskType.BreakChest:
                    newTask = new BreakChestDailyTask(staticTask, dtoTask);
                    break;
                case DailyTaskType.CraftActualIngot:
                    newTask = new CraftActualIngotDailyTask(staticTask, (Dto.ItemDailyTask)dtoTask);
                    break;
                case DailyTaskType.FindChest:
                    newTask = new FindChestDailyTask(staticTask, dtoTask);
                    break;
                case DailyTaskType.PerfectMineComplete:
                    newTask = new PerfectMineCompleteDailyTask(staticTask, dtoTask);
                    break;
                case DailyTaskType.TradeAffairs:
                    newTask = new TradeAffairsDailyTask(staticTask, (Dto.ItemDailyTask)dtoTask);
                    break;
                default:
                    newTask = new DailyTask(staticTask, dtoTask);
                    break;
            }

            Tasks.Add(newTask);
        }
    }
}