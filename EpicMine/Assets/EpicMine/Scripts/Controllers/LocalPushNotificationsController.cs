using System;
using System.Globalization;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class LocalPushNotificationsController
    {
        private readonly int _maxDailyNotificationCount = 15;
        private readonly string _dailyCountKey = "DailyPushCount";
        private readonly string _lastTimeKey = "DailyLastTime";

        private DateTime _lastTimeNotification;
        private int _dailyNotificationCount;

        public bool IsFull => _dailyNotificationCount <= 0;

        public LocalPushNotificationsController()
        {
            Clear();
            Load();
            Schedule();

            EventManager.Instance.Subscribe<WorkshopSlotStartMeltingEvent>(OnMeltingStart);
            EventManager.Instance.Subscribe<WorkshopSlotCompleteEvent>(OnMeltingEnd);

            EventManager.Instance.Subscribe<ChestStartBreakingEvent>(OnStartChestBreaking);
            EventManager.Instance.Subscribe<ChestBreakedEvent>(OnEndChestBreaking);
            EventManager.Instance.Subscribe<BurglarChestOpenedEvent>(OnEndChestBreaking);
            EventManager.Instance.Subscribe<AutoMinerChangeEvent>(OnAutoMinerChange);
        }

        private void Cancel(PushNotificationType type)
        {
            var key = type.ToString();

            if (PlayerPrefs.HasKey(key))
            {
                var taskId = PlayerPrefs.GetString(key);
                App.Instance.Services.LocalPushNotificationsService.Cancel(taskId);
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }
        }

        private void OnMeltingStart(WorkshopSlotStartMeltingEvent eventData)
        {
            AddWorkshopNotifications();
        }

        private void OnMeltingEnd(WorkshopSlotCompleteEvent eventData)
        {
            Cancel(PushNotificationType.WorkShopNotificationSlots);
            Cancel(PushNotificationType.WorkShopNotificationShard);
            AddWorkshopNotifications();
        }

        private void OnStartChestBreaking(ChestStartBreakingEvent eventData)
        {
            AddChestsNotifications();
        }

        private void OnAutoMinerChange(AutoMinerChangeEvent eventData)
        {
            AddAutoMinerNotifications();
        }


        private void OnEndChestBreaking(ChestBreakedEvent eventData)
        {
            Cancel(PushNotificationType.ChestBrokenNotifications);
            AddChestsNotifications();
        }

        private void OnEndChestBreaking(BurglarChestOpenedEvent eventData)
        {
            Cancel(PushNotificationType.ChestBrokenNotifications);
            AddChestsNotifications();
        }

        private void Load()
        {
            _dailyNotificationCount = PlayerPrefs.GetInt(_dailyCountKey, _maxDailyNotificationCount);
            _lastTimeNotification = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            var timeStr = PlayerPrefs.GetString(_lastTimeKey,"");

            if (DateTime.TryParse(timeStr, out _lastTimeNotification))
            {
                var timeLeft = (_lastTimeNotification.AddDays(1) - DateTime.UtcNow);
                if (timeLeft.Hours <= 0)
                {
                    SaveResetNotification();
                }

                return;
            }

            SaveResetNotification();           
        }

        public void SaveResetNotification()
        {
            _dailyNotificationCount = _maxDailyNotificationCount;

            PlayerPrefs.SetInt(_dailyCountKey, _dailyNotificationCount);
            PlayerPrefs.Save();
        }

        private void SaveLocalNotification()
        {
            _dailyNotificationCount = _dailyNotificationCount > 0 ? _dailyNotificationCount - 1 : _dailyNotificationCount;
            _lastTimeNotification = DateTime.Now;

            PlayerPrefs.SetString(_lastTimeKey, _lastTimeNotification.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetInt(_dailyCountKey, _dailyNotificationCount);

            PlayerPrefs.Save();
        }


        public void RewriteTask(string id, PushNotificationType type)
        {
            if (id == "-1")
                return;

            var key = type.ToString();

            var isAlreadyExist = PlayerPrefs.HasKey(key);

            if (isAlreadyExist)
            {
                var taskId = PlayerPrefs.GetString(key);
                App.Instance.Services.LocalPushNotificationsService.Cancel(taskId);
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();

                if (_dailyNotificationCount < _maxDailyNotificationCount)
                    _dailyNotificationCount++;

                 //  App.Instance.Services.LogService.Log($"Push exist key {key} id {taskId}, remove it, left count " + _dailyNotificationCount);
            }

            if (IsFull)
            {
                // App.Instance.Services.LogService.Log("Push loc is full, cancel " + key + ":" + id);
              //  App.Instance.Services.LogService.Log("Push left : " + _dailyNotificationCount);

               App.Instance.Services.LocalPushNotificationsService.Cancel(id);
            }
            else
            {
              //  App.Instance.Services.LogService.Log("Push loc key " + key +" id: " + id);

                PlayerPrefs.SetString(key, id);
                PlayerPrefs.Save();

                SaveLocalNotification();
            }
        }

        public void Schedule()
        {
            if (App.Instance == null || App.Instance.Player == null || App.Instance.Services == null || App.Instance.Services.LocalPushNotificationsService == null)
                return;

            AddChestsNotifications();
            AddWorkshopNotifications();
            AddDailyTasksNotifications();
            AddAbsenceNotifications();
        }

        public void Clear()
        {
            App.Instance.Services.LocalPushNotificationsService.CancelAll();

            var localPushTypes = Enum.GetValues(typeof(PushNotificationType));
            foreach (var localPushType in localPushTypes)
            {
                var type = localPushType.ToString();
                PlayerPrefs.DeleteKey(type);
            }
        }

        private void AddAutoMinerNotifications()
        {
            if (App.Instance.Player.AutoMiner == null)
                return;

            var titleLocale = LocalizationHelper.GetLocale("auto_miner_full_title_local_push_notification");
            var descriptionLocale = LocalizationHelper.GetLocale("auto_miner_full_description_local_push_notification");

            if (App.Instance.Player.AutoMiner.IsFull)
                return;

            var delay = TimeManager.Instance.Now.AddSeconds(App.Instance.Player.AutoMiner.TimeLeftTotal) - TimeManager.Instance.Now;
            var id = App.Instance.Services.LocalPushNotificationsService.Push(titleLocale, descriptionLocale, delay);

            RewriteTask(id, PushNotificationType.AutoMinerChangeNotifications);
        }


        private void AddChestsNotifications()
        {
            if (App.Instance.Player.Burglar == null)
                return;

            var titleLocale = LocalizationHelper.GetLocale("chest_broken_title_local_push_notification");
            var descriptionLocale = LocalizationHelper.GetLocale("chest_broken_description_local_push_notification");

            foreach (var burglarChest in App.Instance.Player.Burglar.Chests)
            {
                if (!burglarChest.IsBreakingStarted)
                    continue;

                if (burglarChest.IsBroken)
                    continue;

                var delay = StaticHelper.GetChestBreakingTimeLeft(burglarChest.Type, burglarChest.StartBreakingTime.Value);

                var id = App.Instance.Services.LocalPushNotificationsService.Push(titleLocale, descriptionLocale, delay);

                RewriteTask(id, PushNotificationType.ChestBrokenNotifications);
            }
        }

        public void AddWorkshopNotifications()
        {
            if (App.Instance.Player.Workshop == null)
                return;

            var titleLocale = LocalizationHelper.GetLocale("workshop_smelting_completed_title_local_push_notification");
            var descriptionLocale = LocalizationHelper.GetLocale("workshop_smelting_completed_description_local_push_notification");

            TimeSpan? delay = null;

            foreach (var workshopSlot in App.Instance.Player.Workshop.Slots)
            {
                if (!workshopSlot.IsUnlocked)
                    continue;

                if (workshopSlot.StaticRecipe == null)
                    continue;

                if (workshopSlot.IsComplete)
                    continue;

                if (workshopSlot.CompleteAmount == workshopSlot.NecessaryAmount)
                    continue;

                if (delay == null || delay < workshopSlot.FullAmountTimeLeft)
                    delay = workshopSlot.FullAmountTimeLeft;
            }

            if (delay != null)
            {
                var id = App.Instance.Services.LocalPushNotificationsService.Push(titleLocale, descriptionLocale, delay.Value);

                RewriteTask(id, PushNotificationType.WorkShopNotificationSlots);
            }

            delay = null;

            foreach (var workshopSlot in App.Instance.Player.Workshop.SlotsShard)
            {
                if (!workshopSlot.IsUnlocked)
                    continue;

                if (workshopSlot.StaticRecipe == null)
                    continue;


                if (workshopSlot.CompleteAmount == workshopSlot.NecessaryAmount)
                    continue;

                if (workshopSlot.IsComplete)
                    continue;

                if (delay == null || delay < workshopSlot.FullAmountTimeLeft)
                    delay = workshopSlot.FullAmountTimeLeft;
            }

            if (delay != null)
            {
                var id = App.Instance.Services.LocalPushNotificationsService.Push(titleLocale, descriptionLocale, delay.Value);

                RewriteTask(id, PushNotificationType.WorkShopNotificationShard);
            }
        }

        private void AddDailyTasksNotifications()
        {
            if (App.Instance.Controllers?.DailyTasksController == null)
                return;

            var titleLocale = LocalizationHelper.GetLocale("daily_tasks_update_title_local_push_notification");
            var descriptionLocale = LocalizationHelper.GetLocale("daily_tasks_update_description_local_push_notification");

            foreach (var dailyTask in App.Instance.Controllers.DailyTasksController.Tasks)
            {
                if (!dailyTask.IsCompleted || !dailyTask.IsRewardTaken)
                    continue;

                var tomorrow = DateTime.Today.AddDays(1).ToUniversalTime();
                var delay = tomorrow - DateTime.Now;

                var id = App.Instance.Services.LocalPushNotificationsService.Push(titleLocale, descriptionLocale, delay);
                
                RewriteTask(id, PushNotificationType.DailyTasksNotifications);
            
                break;
            }
        }

        private void AddAbsenceNotifications()
        {
            var oneDayTitle = LocalizationHelper.GetLocale("one_day_absence_title_local_push_notification");
            var oneDayDescription = LocalizationHelper.GetLocale("one_day_absence_description_local_push_notification");
            var oneDayDelay = DateTime.Now.AddDays(1) - DateTime.Now;

            var sevenDaysTitle = LocalizationHelper.GetLocale("seven_days_absence_title_local_push_notification");
            var sevenDaysDescription = LocalizationHelper.GetLocale("seven_days_absence_description_local_push_notification");
            var sevenDaysDelay = DateTime.Now.AddDays(7) - DateTime.Now;

            var id = App.Instance.Services.LocalPushNotificationsService.Push(oneDayTitle,oneDayDescription, oneDayDelay);
            var id2 = App.Instance.Services.LocalPushNotificationsService.Push(sevenDaysTitle,sevenDaysDescription, sevenDaysDelay);

            RewriteTask(id, PushNotificationType.AbsenceNotificationShort);
            RewriteTask(id2, PushNotificationType.AbsenceNotificationLong);
        }
    }
}