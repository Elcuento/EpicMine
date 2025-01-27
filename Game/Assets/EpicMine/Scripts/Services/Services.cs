using BlackTemple.Common;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Services;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class Services
    {
        public ILogService LogService { get; }

        public IStorageService RuntimeStorage { get; }

        public IAnalyticsService AnalyticsService { get; }

        public IAdvertisementService AdvertisementService { get; }

        public ILocalPushNotificationService LocalPushNotificationsService { get; }

     //   public TenjinServiceAdapter TenjinServiceAdapter { get; }


        public Services()
        {
            var isDebug = Debug.isDebugBuild || Application.version == "0.0.0";

            if (isDebug)
            {
                LogService =  new DebugLogService();
                AnalyticsService = new EmptyAnalyticsService();
            }
            else
            {
                AnalyticsService = new EmptyAnalyticsService();
                LogService = new EmptyLogService();

                /* var tenjin = new GameObject("TenjinService");
                 tenjin.transform.SetParent(App.Instance.transform);
                 TenjinServiceAdapter = tenjin.AddComponent<TenjinServiceAdapter>();*/

                /*   Object.Instantiate(Resources.Load<GameAnalytics>("GameAnalytics"));
                   AnalyticsService = new DevToDevAnalyticsServiceAdapter();*/
            }

            RuntimeStorage = new RuntimeStorageService();
            LocalPushNotificationsService = new UnityLocalPushNotificationServiceAdapter();

            var appodealGo = new GameObject("AdvertisementService");
            appodealGo.transform.SetParent(App.Instance.transform);
            AdvertisementService = appodealGo.AddComponent<AppodealAdvertisementServiceAdapter>();
        }
    }
}