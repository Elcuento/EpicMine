using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public class NotificationsController
    {
        private bool _isTutorialEnded;

        public NotificationsController()
        {
            EventManager.Instance.Subscribe<ChestBreakedEvent>(OnChestBroken);
            EventManager.Instance.Subscribe<WorkshopSlotCompleteEvent>(OnWorkshopSlotComplete);
            EventManager.Instance.Subscribe<ArtefactsAmountChangeEvent>(OnArtefactsAmountChange);
            EventManager.Instance.Subscribe<DailyTaskCompleteEvent>(OnDailyTaskComplete);
            EventManager.Instance.Subscribe<AutoMinerFullEvent>(OnAutoMinerFullEvent);
        }

        private void SubscribeQuests()
        {
            EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnTutorialCompleted);
            EventManager.Instance.Subscribe<QuestActivateEvent>(OnQuestActivateEvent);
            EventManager.Instance.Subscribe<QuestStartEvent>(OnQuestStartEvent);
            EventManager.Instance.Subscribe<QuestTaskGoalCompleteEvent>(OnQuestTaskGoalComplete);
        }

        public void Initialize()
        {
            _isTutorialEnded = App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks);

            if (!_isTutorialEnded)
                EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnTutorialCompleted);
            else SubscribeQuests();
        }

        private void OnTutorialCompleted(TutorialStepCompleteEvent eventData)
        {
            _isTutorialEnded = eventData.Step.Id >= TutorialStepIds.ShowDailyTasks;

            if (_isTutorialEnded)
            {
                SubscribeQuests();
            }
        }

        private void OnQuestStartEvent(QuestStartEvent eventData)
        {
            if (eventData.Quest.StaticQuest.ActivateTriggerType != QuestTriggerType.Speak
                && eventData.Quest.StaticQuest.StartTriggerType != QuestTriggerType.Speak)
                return;

            var window = WindowManager.Instance.Show<WindowNotifications>(withSound: false);
            window.ShowNotification(NotificationType.QuestStart);
        }

        private void OnQuestActivateEvent(QuestActivateEvent eventData)
        {
            if (eventData.Quest.StaticQuest.ActivateTriggerType != QuestTriggerType.Speak
                && eventData.Quest.StaticQuest.StartTriggerType != QuestTriggerType.Speak)
                return;

            var window = WindowManager.Instance.Show<WindowNotifications>(withSound: false);
            window.ShowNotification(NotificationType.QuestActivate);
        }

        private void OnQuestTaskGoalComplete(QuestTaskGoalCompleteEvent eventData)
        {
            var window = WindowManager.Instance.Show<WindowNotifications>(withSound: false);
            window.ShowNotification(NotificationType.QuestChange);
        }

        private void OnAutoMinerFullEvent(AutoMinerFullEvent eventData)
        {
            var window = WindowManager.Instance.Show<WindowNotifications>(withSound: false);
            window.ShowNotification(NotificationType.AutoMinerFull);
        }

        private void OnDailyTaskComplete(DailyTaskCompleteEvent eventData)
        {
            var window = WindowManager.Instance.Show<WindowNotifications>(withSound: false);
            window.ShowNotification(eventData.Task);
        }

        private void OnArtefactsAmountChange(ArtefactsAmountChangeEvent eventData)
        {
          //  var nextTier = App.Instance.Player.Dungeon.Tiers.FirstOrDefault(t => t.IsOpen == false);
           // if (nextTier == null)
            //    return;

         //   if (eventData.Amount < nextTier.StaticTier.RequireArtefacts)
           //     return;

           if (eventData.Amount < LocalConfigs.MaxArtefacts)
               return;
            
            var window = WindowManager.Instance.Show<WindowNotifications>(withSound: false);
            window.ShowNotification(NotificationType.ArtefactsCollected);
        }

        private void OnWorkshopSlotComplete(WorkshopSlotCompleteEvent eventData)
        {
            var window = WindowManager.Instance.Show<WindowNotifications>(withSound: false);
            window.ShowNotification(eventData.WorkshopSlot);
        }

        private void OnChestBroken(ChestBreakedEvent eventData)
        {
            var window = WindowManager.Instance.Show<WindowNotifications>(withSound: false);
            window.ShowNotification(NotificationType.ChestBreakingCompleted);
        }


    }
}