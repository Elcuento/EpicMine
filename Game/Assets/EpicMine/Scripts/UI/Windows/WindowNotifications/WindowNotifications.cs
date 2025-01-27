using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowNotifications : WindowBase
    {
        [SerializeField] private WindowNotificationsNotification _notificationPrefab;
        [SerializeField] private Transform _notificationsContainer;
        
        
        public void ShowNotification(DailyTask dailyTask)
        {
            var notification = InstantiateNotification();
            notification.Initialize(dailyTask);
        }

        public void ShowNotification(Core.WorkshopSlot workshopSlot)
        {
            var notification = InstantiateNotification();
            notification.Initialize(workshopSlot);
        }

        public void ShowNotification(NotificationType notificationType)
        {
            var notification = InstantiateNotification();
            notification.Initialize(notificationType);
        }


        private WindowNotificationsNotification InstantiateNotification()
        {
            var notification = Instantiate(_notificationPrefab, _notificationsContainer, false);
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Notification);
            return notification;
        }
    }
}