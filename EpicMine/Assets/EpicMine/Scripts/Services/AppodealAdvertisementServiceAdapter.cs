using System;
using System.Collections;
using System.Collections.Generic;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using BlackTemple.Common;
using UnityEngine;
using EventHandler = BlackTemple.Common.EventHandler;

namespace BlackTemple.EpicMine
{
    public class AppodealAdvertisementServiceAdapter : MonoBehaviour, IAdvertisementService, IRewardedVideoAdListener, IInterstitialAdListener
    {
        public event EventHandler<AdSource, bool, string, int> OnRewardedVideoCompleted;
        public event EventHandler OnInterstitialCompleted;

        private const string SpeedupChestBreakingPlacement = "speedup_chest_breaking";

        private const string RefillPickaxeHealthPlacement = "refill_pickaxe_health";

        private const string MultiplySmeltedResourcesPlacement = "multiply_smelted_resources";

        private const string OpenEnchantedChest = "open_enchanted_chest";

        private const string UnlockPickaxe = "unlock_pickaxe";

        private const string UnlockTorch = "unlock_torch";

        private const string OpenPvpChest = "open_pvp_chest";

        private const string AdCrystalReward = "ad_crystal_reward";

        private const string AdGoldReward = "ad_gold_reward";

        private AdSource _adSource;

        private string _placement;

        private string _rewardName;

        private int _rewardAmount;

        private bool _isRewardedVideoCompleted;

        private bool _isRewardedVideoShowed;

        private bool _isInterstitialCompleted;

        private long _nextTimeForceAdsShowed;

        private int _nextForceAdsPeriod;

        private bool _isForceAdsDisable;

        private bool _isForceAdsCooldown;

        public void Initialize(string userId = "")
        {
            string appKey = "";

#if UNITY_ANDROID
            appKey = Application.identifier == "ru.blacktemple.epicmine"
                ? "59c7f4e744877885740e56dc1c5b9b5ba5b5a203806dd706"
                : "750e291f2d08a2b1a88c55d433c165e70fd1e87bddb5302c";
#elif UNITY_IOS || UNITY_IPHONE
            appKey = Application.identifier == "ru.blacktemple.epicmine"
                ? "5557d40d9f6cd1de9f0818c5d657326b18441e5cd7b95747"
                : "dd1b557b842007ae6af87489067fee9ab042d4877ec33977";
#endif

            Appodeal.disableLocationPermissionCheck();
               //   Appodeal.disableWriteExternalStoragePermissionCheck();

           Appodeal.setLogLevel(Debug.isDebugBuild ? Appodeal.LogLevel.Verbose : Appodeal.LogLevel.None);

         //   Appodeal.setLogLevel(Appodeal.LogLevel.Verbose);

            if (!string.IsNullOrEmpty(userId))
            { 
               // var appodealUserSettings = new UserSettings();
                Appodeal.setUserId(userId);
                //appodealUserSettings.setUserId(userId);
            }

           // Appodeal.disableNetwork("vungle");
            Appodeal.initialize(appKey, Appodeal.REWARDED_VIDEO | Appodeal.INTERSTITIAL);
            Appodeal.setRewardedVideoCallbacks(this);
            Appodeal.setInterstitialCallbacks(this);

            _nextTimeForceAdsShowed = 0;
            _nextForceAdsPeriod = App.Instance.StaticData.Configs.Ads.AdsShopOfferPeriod;
        }

        public void DisableForceAds(bool state)
        {
            _isForceAdsDisable = state;
        }

        public bool IsForceAdsAvailable()
        {
            if (!App.Instance.StaticData.Configs.Ads.AdsInternalShow)
                return false;

            return !_isForceAdsDisable && App.Instance.Player.Dungeon.LastOpenTierNumber < App.Instance.StaticData.Configs.Ads.AdsTierDisable && _nextTimeForceAdsShowed < TimeManager.Instance.NowUnixSeconds;
        }

        public int GetCurrentForceAdsPeriod()
        {
            return _nextForceAdsPeriod;
        }

        public void ShowForceAds(bool withCoolDown = true)
        {
            if (!App.Instance.StaticData.Configs.Ads.AdsInternalShow)
                return;

            if (_isForceAdsDisable)
                return;

            if (_isForceAdsCooldown)
            {
                if (!IsForceAdsAvailable())
                    return;
            }

            ShowInterstitialVideo(withCoolDown);
        }

        public void ShowRewardedVideo(AdSource adSource)
        {
            Clear();
            _adSource = adSource;

            switch (_adSource)
            {
                case AdSource.SpeedupChestBreaking:
                    _placement = SpeedupChestBreakingPlacement;
                    break;
                case AdSource.RefillPickaxeHealth:
                    _placement = RefillPickaxeHealthPlacement;
                    break;
                case AdSource.MultiplySmeltedResources:
                    _placement = MultiplySmeltedResourcesPlacement;
                    break;
                case AdSource.OpenEnchantedChest:
                    _placement = OpenEnchantedChest;
                    break;
                case AdSource.UnlockPickaxe:
                    _placement = UnlockPickaxe;
                    break;
                case AdSource.UnlockTorch:
                    _placement = UnlockTorch;
                    break;
                case AdSource.MultiplyPvpChestReward:
                    _placement = OpenPvpChest;
                    break;
                case AdSource.CrystalAdReward:
                    _placement = AdCrystalReward;
                    break;
                case AdSource.GoldAdReward:
                    _placement = AdGoldReward;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_adSource), _adSource, null);
            }

            StartCoroutine(ShowRewardedVideoInternal());
        }

        public void ShowInterstitialVideo(bool withCoolDown = true)
        {
            Clear();

            _isForceAdsCooldown = withCoolDown;

            StartCoroutine(ShowInterstitialVideoInternal());
        }

        public void onRewardedVideoLoaded(bool preCache)
        {
            App.Instance.Services.LogService.Log("Rewarded video loaded");
        }

        public void onRewardedVideoFailedToLoad()
        {
            App.Instance.Services.LogService.Log("Rewarded video failed to load");
            _isRewardedVideoCompleted = true;
        }

        public void onRewardedVideoShown()
        {
            App.Instance.Services.LogService.Log("Rewarded video shown");
        }

        public void onRewardedVideoFinished(double rewardAmount, string rewardName)
        {
            App.Instance.Services.LogService.Log($"Rewarded video finished, reward name: {rewardName}, amount: {rewardAmount}");
            _rewardName = rewardName;
            _rewardAmount = (int)rewardAmount;

            App.Instance.Services.AnalyticsService.CustomEvent("rewarded_video_finished", new CustomEventParameters
            {
                String = new Dictionary<string, string>
                {
                    { _placement, _adSource.ToString() },
                }
            });
        }

        public void onRewardedVideoClosed(bool isShowed)
        {
            App.Instance.Services.LogService.Log($"Rewarded video closed, isShowed: {isShowed}");
            _isRewardedVideoShowed = isShowed;
            _isRewardedVideoCompleted = true;

            App.Instance.Services.AnalyticsService.CustomEvent("rewarded_video_closed", new CustomEventParameters
            {
                String = new Dictionary<string, string>
                {
                    { _placement, _adSource.ToString() },
                    { "Showed", isShowed.ToString() }
                }
            });
        }

        public void onRewardedVideoExpired()
        {
            App.Instance.Services.LogService.Log("Rewarded video expired");
            _isRewardedVideoCompleted = true;
        }


        private IEnumerator ShowInterstitialVideoInternal()
        {
            WindowManager.Instance.Show<WindowPreloader>(withSound: false);

            var timeOut = DateTime.Now.AddSeconds(15);
            
            try
            {
#if UNITY_EDITOR
             _isInterstitialCompleted = true;
            // Debug.Log(_isForceAdsCooldown);
             if (_isForceAdsCooldown)
             {
                 _nextTimeForceAdsShowed = TimeManager.Instance.NowUnixSeconds +
                                           App.Instance.StaticData.Configs.Ads.AdsCoolDown;
            //     Debug.Log(_nextTimeForceAdsShowed);
             }
#else
           if (NetworkManager.Instance.IsInternetAvailable && Appodeal.isLoaded(Appodeal.INTERSTITIAL))
             { 
                Appodeal.show(Appodeal.INTERSTITIAL, "force_ads");
                _nextForceAdsPeriod++;
                if(_isForceAdsCooldown)
                _nextTimeForceAdsShowed = TimeManager.Instance.NowUnixSeconds + App.Instance.StaticData.Configs.Ads.AdsCoolDown;
             }
            else
             {
                Debug.Log("Video not loaded");
                _isInterstitialCompleted = true;
             }
#endif
            }
            catch (Exception e)
            {
                _isInterstitialCompleted = true;
                App.Instance.Services.LogService.LogError("Error on loading appodealvideo " + e);
            }

            yield return new WaitUntil(() => _isInterstitialCompleted || timeOut < DateTime.Now);

            if (!_isInterstitialCompleted)
            {
                WindowManager
                    .Instance
                    .Show<WindowAlert>(withSound: false)
                    .Initialize("rewarded_video_failed_to_load");
            }

            WindowManager.Instance.Close<WindowPreloader>(withSound: false);

            OnInterstitialCompleted?.Invoke();
        }


        private IEnumerator ShowRewardedVideoInternal()
        {
            WindowManager.Instance.Show<WindowPreloader>(withSound: false);

            var timeOut = DateTime.Now.AddSeconds(50);

            try
            {
#if UNITY_EDITOR
                _isRewardedVideoShowed = true;
                _isRewardedVideoCompleted = true;
#else
            if (NetworkManager.Instance.IsInternetAvailable && Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
                Appodeal.show(Appodeal.REWARDED_VIDEO, _placement);
            else
                _isRewardedVideoCompleted = true;
#endif
            }
            catch (Exception e)
            {
                App.Instance.Services.LogService.LogError("Error on loading appodealvideo " + e);
                _isRewardedVideoCompleted = true;
                _isRewardedVideoShowed = false;
            }

            yield return new WaitUntil(() => _isRewardedVideoCompleted || timeOut < DateTime.Now);

            if (!_isRewardedVideoShowed)
            {
                WindowManager
                    .Instance
                    .Show<WindowAlert>(withSound: false)
                    .Initialize("rewarded_video_failed_to_load");
            }
            // artificial delay, for response from appodeal to firebase and not get timeout error
            else if (_adSource == AdSource.SpeedupChestBreaking || _adSource == AdSource.OpenEnchantedChest
                                                                || _adSource == AdSource.CrystalAdReward
                                                                || _adSource == AdSource.GoldAdReward)
            {
                yield return new WaitForSecondsRealtime(3f);
            }

            WindowManager.Instance.Close<WindowPreloader>(withSound: false);

            OnRewardedVideoCompleted?.Invoke(_adSource, _isRewardedVideoShowed, _rewardName, _rewardAmount);
        }

        private void Clear()
        {
            StopAllCoroutines();
            _placement = string.Empty;

            _rewardName = string.Empty;
            _rewardAmount = 0;

            _isRewardedVideoCompleted = false;
            _isRewardedVideoShowed = false;
            _isInterstitialCompleted = false;

            _isForceAdsCooldown = false;
        }

        public void onRewardedVideoClicked()
        {
            App.Instance.Services.LogService.Log("rewarded video clicked");
            App.Instance.Services.AnalyticsService.CustomEvent("rewarded_video_clicked", new CustomEventParameters
            {
                String = new Dictionary<string, string>
                {
                    { _placement, _adSource.ToString() }
                }
            });
        }

        public void onInterstitialLoaded(bool isPrecache)
        {
            //throw new NotImplementedException();
        }

        public void onInterstitialFailedToLoad()
        {
            _isInterstitialCompleted = true;
        }

        public void onInterstitialShown()
        {
            App.Instance.Services.LogService.Log("Interstitial shown");
            _isInterstitialCompleted = true;
            App.Instance.Services.AnalyticsService.CustomEvent("interstitial_showed", new CustomEventParameters());
        }

        public void onInterstitialClosed()
        {
            App.Instance.Services.LogService.Log($"Interstitial closed , period {_nextForceAdsPeriod}/{App.Instance.StaticData.Configs.Ads.AdsShopOfferPeriod}");
            App.Instance.Services.AnalyticsService.CustomEvent("interstitial_closed", new CustomEventParameters());
            _isInterstitialCompleted = true;

            if (_nextForceAdsPeriod >= App.Instance.StaticData.Configs.Ads.AdsShopOfferPeriod)
                _nextForceAdsPeriod = 0;
            else _nextForceAdsPeriod++;
        }

        public void onInterstitialClicked()
        {
          App.Instance.Services.AnalyticsService.CustomEvent("interstitial_clicked", new CustomEventParameters());
        }

        public void onInterstitialExpired()
        {
            _isInterstitialCompleted = true;

        }

        public void onInterstitialShowFailed()
        {
            _isInterstitialCompleted = true;
        }

        public void onRewardedVideoShowFailed()
        {
            _isRewardedVideoCompleted = true;
        }
    }
}