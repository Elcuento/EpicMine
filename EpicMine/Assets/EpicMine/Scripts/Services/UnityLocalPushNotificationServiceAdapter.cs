using System;
using System.Threading.Tasks;
#if UNITY_IPHONE || UNITY_IOS
using Unity.Notifications.iOS;    
#else
using Unity.Notifications.Android;    
#endif
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine.Assets.EpicMine.Scripts.Services
{
    // ReSharper disable once IdentifierTypo
    public class UnityLocalPushNotificationServiceAdapter : ILocalPushNotificationService
    {
        private string _channelId = "0";

        public void Initialize()
        {
            _channelId = Random.Range(0, 99999).ToString();

#if UNITY_ANDROID
            var c = new AndroidNotificationChannel(_channelId, "Epic Mine", "None", Importance.High);
            AndroidNotificationCenter.RegisterNotificationChannel(c);
#elif UNITY_IPHONE || UNITY_IOS
         AsyncAuthorizationRequest();
#endif
        }

#if UNITY_IPHONE || UNITY_IOS
        private async void AsyncAuthorizationRequest()
        {
                using (var req = new AuthorizationRequest(
                    AuthorizationOption.Alert | AuthorizationOption.Badge, true))
                {
                    while (!req.IsFinished)
                    {
                        await Task.Delay(1000);
                    }

                    var res = "\n RequestAuthorization: \n";
                    res += "\n finished: " + req.IsFinished;
                    res += "\n granted :  " + req.Granted;
                    res += "\n error:  " + req.Error;
                    res += "\n deviceToken:  " + req.DeviceToken;
                    Debug.Log(res);
                }
         }
        
#endif


        public string Push(string title, string text, DateTime time)
        {
            var notId = "-1";

            if (DateTime.Now > time)
                return notId;

          //  App.Instance.Services.LogService.Log($"Push notification {title},{time} fire at {time}");


#if UNITY_ANDROID

            var androidNotification = new AndroidNotification
            {
                Title = title,
                Text = text,
                FireTime = time,
                SmallIcon = "icon_small",
            };

            notId = AndroidNotificationCenter.SendNotification(androidNotification, _channelId)
                   .ToString();

#elif UNITY_IPHONE || UNITY_IOS

            notId = Random.Range(-99999999, 99999999).ToString();
            var iosTime = time - DateTime.Now;


            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = iosTime,
                Repeats = false,
            };

            //App.Instance.Services.LogService.Log($"Push at {iosTime}");

            var iosNotification = new iOSNotification()
            {
                Identifier = notId,
                Title = title,
                Body = text,
                Subtitle = "",
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert |
                                                PresentationOption.Sound),
                CategoryIdentifier = "category_a",
                ThreadIdentifier = "thread1",
                Trigger = timeTrigger,
            };

            notId = iosNotification.Identifier;

            iOSNotificationCenter.ScheduleNotification(iosNotification);
#endif

            return notId;
        }

        public string Push(string title, string text, TimeSpan time)
        {
            var notId = "-1";

            if (time < TimeSpan.Zero)
                return notId;

            //App.Instance.Services.LogService.Log($"Push notification {title},{time} fire at {time}");

#if UNITY_ANDROID

            var androidDate = DateTime.Now + time;

            var androidNotification = new AndroidNotification
            {
                Title = title,
                Text = text,
                FireTime = androidDate,
                SmallIcon = "icon_small",
            };
            notId = AndroidNotificationCenter.SendNotification(androidNotification, _channelId).ToString();
            //App.Instance.Services.LogService.Log(notId +":" + title + ":" + text + " fire at " + androidNotification.FireTime);

#elif UNITY_IPHONE || UNITY_IOS

            notId = Random.Range(-99999999, 99999999).ToString();

            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = time,
                Repeats = false,
            };

            var iosNotification = new iOSNotification()
            {
                Identifier = notId,
                Title = title,
                Body = text,
                Subtitle = "",
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert |
                                                PresentationOption.Sound),
                CategoryIdentifier = "category_a",
                ThreadIdentifier = "thread1",
                Trigger = timeTrigger,
            };

            notId = iosNotification.Identifier;

            iOSNotificationCenter.ScheduleNotification(iosNotification);
#endif
            return notId;
        }



        public void Cancel(string notId)
        {
            if (int.TryParse(notId, out var idNumber))
            {
#if UNITY_ANDROID
            //App.Instance.Services.LogService.Log(idNumber + " cancel ");
            AndroidNotificationCenter.CancelNotification(idNumber);
#elif UNITY_IPHONE || UNITY_IOS
            iOSNotificationCenter.RemoveScheduledNotification(notId);
#endif  
            }
        }

        public void CancelAll()
        {

#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IPHONE || UNITY_IOS
          iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif  
        }
    }

}
