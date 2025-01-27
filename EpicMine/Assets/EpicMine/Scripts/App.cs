using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Utils;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using GameEvent = CommonDLL.Dto.GameEvent;
using GameEvents = BlackTemple.EpicMine.Core.GameEvents;
using Player = BlackTemple.EpicMine.Core.Player;
using SystemInfo = UnityEngine.Device.SystemInfo;


namespace BlackTemple.EpicMine
{
    public class App : Singleton<App>
    {
        [SerializeField] private TextAsset _staticData;
        [SerializeField] private TextAsset[] _localizationData;

        public StaticData StaticData { get; private set; }
        public LocalizationData[] LocalizationData;//{ get; private set; }
        public LocalizationData CurrentLocalizationData;// { get; private set; }
        public Player Player { get; private set; }

        public AppVersionInfo VersionInfo { get; private set; }

        public GameEvents GameEvents { get; private set; }

        public Services Services { get; private set; }

        public Controllers Controllers { get; private set; }

        public ReferencesTables ReferencesTables;

        public bool IsInitialized { get; set; }

        public LocalizationData GetLocalizationData(SystemLanguage aLanguage)
        {
            foreach (var localizationData in LocalizationData)
            {
                if (localizationData.Code == aLanguage.ToString())
                    return localizationData;
            }

            return LocalizationData[0];
        }
        public PlatformType CurrentPlatform => Application.platform == RuntimePlatform.IPhonePlayer
            ? PlatformType.IOS : Application.platform == RuntimePlatform.Android ? PlatformType.Android : PlatformType.All;
     
        public static string DeviceId => SystemInfo.deviceUniqueIdentifier;
     //   public static string TempId;


     public void SetLanguage(string language)
     {
         var all = Enum.GetValues(typeof(SystemLanguage));

         for (var i = 0; i < all.Length; i++)
         {
             var lan = (SystemLanguage)i;

             if (lan.ToString() == language)
             {
                 SetLanguage(lan);
                 return;
             }
         }

         SetLanguage(SystemLanguage.English);
        }
        public void SetLanguage(SystemLanguage language)
        {

            if (Player == null)
            {

                CurrentLocalizationData = GetLocalizationData(language);

                var all = FindObjectsOfType<SimpleTextLocalizator>();
                foreach (var text in all)
                {
                    text.Refresh();
                }
            }
            else
            {

              //  if (Player.Language != language.ToString())
                {
 
                    CurrentLocalizationData = GetLocalizationData(language);

                    var all = FindObjectsOfType<SimpleTextLocalizator>();
                    foreach (var text in all)
                    {
                        text.Refresh();
                    }
                }
            }

            if (Player != null)
            {
                Player.SetLanguage(language.ToString());
            }

        }

        public void InitData()
        {
            StaticData = _staticData.text.FromJson<StaticData>();

            LocalizationData = new LocalizationData[_localizationData.Length];

            for (var index = 0; index < _localizationData.Length; index++)
            {
                LocalizationData[index] = _localizationData[index].text.FromJson<LocalizationData>();
            }

            SetLanguage(SystemLanguage.English);
        }

        private IEnumerator SaveProfileCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(3);

                Player.Save();
            }
           
        }
        public void Initialize(Player player)
        {
            IsInitialized = true;

            GameEvents = new GameEvents(new List<GameEvent>());

            Player = player;
            
            if (string.IsNullOrEmpty(player.Language))
            {
                player.SetLanguage("English");
            }

            SetLanguage(player.Language);

            Application.targetFrameRate = 60;

            Services.LocalPushNotificationsService.Initialize();
            Services.AdvertisementService.Initialize(Player.Id);
            InAppPurchaseManager.Instance.Initialize(StaticData.Configs.Shop.Iaps);

            Controllers = new Controllers();

            GameEvents.Initialize();

            player.RecalculateTimeSensitive();
            player.Effect.BroadCastEffects();
            player.Quests.Initialize();
            player.AutoMiner.Initialize();

            StartCoroutine(SaveProfileCoroutine());

        }



        public void Restart(bool saveControllers = true)
        {
            EventManager.Instance.Publish(new ClearAppEvent());

            if(saveControllers)
            Controllers?.Save();

            EventManager.Instance.Publish(new ClearAppEvent());

            SceneManager.Instance.LoadScene(ScenesNames.Clear);
        }

        protected override void Awake()
        {
            base.Awake();
            InitData();
            Services = new Services();
        }


#if UNITY_EDITOR
        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            Controllers?.Save();
        }
#endif

        private void OnApplicationPause(bool isPaused)
        {
#if !UNITY_EDITOR
            if (isPaused)
                Controllers?.Save();
#endif

            if (!isPaused)
                Player?.RecalculateTimeSensitive();
        }

    }
}