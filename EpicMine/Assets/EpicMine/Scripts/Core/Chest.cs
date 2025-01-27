using System;
using System.Collections.Generic;
using System.Data;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine.Core
{
    public class Chest
    {
        public string Id { get; }
        public ChestType Type { get; }
        public int Level { get; }
        public DateTime? StartBreakingTime { get; private set; }

        public bool IsBreakingStarted => StartBreakingTime.HasValue;

        public bool IsBroken
        {
            get
            {
                if (!IsBreakingStarted)
                    return false;

                var timeLeft = StaticHelper.GetChestBreakingTimeLeft(Type, StartBreakingTime.Value);
                return timeLeft.TotalSeconds <= 0;
            }
        }

        public Chest(string id, ChestType type, int level, DateTime? time)
        {
            Id = id;
            Type = type;
            Level = level;
            StartBreakingTime = time;

            if (IsBreakingStarted && !IsBroken)
                Subscribe();
        }


        public Chest(string id, Dto.Chest chest)
        {
            Id = id;
            Type = chest.Type;
            Level = chest.Level;
            StartBreakingTime = chest.StartBreakingTime;

            if (IsBreakingStarted && !IsBroken)
                Subscribe();
        }


        public void StartBreaking(Action<bool> onComplete = null)
        {

            foreach (var chest in App.Instance.Player.Burglar.Chests)
            {
                if (chest.StartBreakingTime != null)
                {
                    Debug.LogError("Some chest already breaking");
                    return;
                }
            }

            var chestData = App.Instance.Player.Burglar.Chests.Find(x => x.Id == Id);

            if (chestData == null)
            {
                Debug.LogError("Chest not exist");
                return;
            }

            chestData.StartBreakingTime = DateTime.UtcNow;

            StartBreakingTime = TimeManager.Instance.Now;
            var startBreakingEvent = new ChestStartBreakingEvent(this);
            EventManager.Instance.Publish(startBreakingEvent);
            Subscribe();
            onComplete?.Invoke(true);

            var parameters = new CustomEventParameters
                { Int = new Dictionary<string, int> { { "type", (int)Type }, { "level", Level } } };
            App.Instance.Services.AnalyticsService.CustomEvent("start_chest_breaking", parameters);
        }

        public void Open(Action<bool, int, int> onComplete = null)
        {
            var staticData = App.Instance.StaticData;

            var chestData = App.Instance.Player.Burglar.Chests.Find(x => x.Id == Id);
            if (chestData == null)
            {
                Debug.LogError("Chest not exist");
                return;
            }

            var chestType = chestData.Type;
            var userChestStartBreakingTimeData = chestData.StartBreakingTime;

            // force open chest who not started breaking
            if (userChestStartBreakingTimeData == null)
            {
                Debug.LogError("Chest data not exist");
                return;
            }

            var open = App.Instance.Player.Burglar.OpenChest(staticData.Configs, chestType, chestData);

            var openedEvent = new BurglarChestOpenedEvent(this);
            EventManager.Instance.Publish(openedEvent);
            onComplete?.Invoke(true, open.Item1, open.Item2);

            Unsubscribe();

            var parameters = new CustomEventParameters { Int = new Dictionary<string, int> { { "type", (int)Type }, { "level", Level } } };
            App.Instance.Services.AnalyticsService.CustomEvent("open_chest", parameters);
        }

        public void ForceOpen(Action<bool, long, long, long> onComplete = null, bool isTutorial = false)
        {
            if (isTutorial)
            {
                var staticData = App.Instance.StaticData;

                var chest = App.Instance.Player.Burglar.Chests.Find(x => x.Id == Id);
                if (chest == null)
                {
                    Debug.LogError("Second open tutorial chest");

                }

                var res = App.Instance.Player.Burglar.OpenSpecificChestInternal(staticData.Configs, chest);

                var openedEvent = new BurglarChestOpenedEvent(this);
                EventManager.Instance.Publish(openedEvent);
                onComplete?.Invoke(true, 0, res.Item1,
                    res.Item2);

                Unsubscribe();

                var parameters = new CustomEventParameters
                {
                    Int = new Dictionary<string, int>
                    {
                        { "type", (int)Type },
                        { "level", Level },
                        { "spent_crystals", 0 },
                        { "dropped_crystals", (int)res.Item1 },
                        { "dropped_artefacts", (int)res.Item2 }
                    }
                };
                App.Instance.Services.AnalyticsService.CustomEvent("force_open_chest", parameters);
            }
            else
            {

                void OpenResult(long cost, long droppedCrystals, long droppedArtefacts)
                {
                    var openedEvent = new BurglarChestOpenedEvent(this);
                    EventManager.Instance.Publish(openedEvent);
                    onComplete?.Invoke(true, cost, droppedCrystals,
                        droppedArtefacts);

                    Unsubscribe();

                    var parameters = new CustomEventParameters
                    {
                        Int = new Dictionary<string, int>
                        {
                            { "type", (int)Type },
                            { "level", Level },
                            { "spent_crystals", (int)cost },
                            { "dropped_crystals", (int)droppedCrystals },
                            { "dropped_artefacts", (int)droppedArtefacts }
                        }
                    };
                    App.Instance.Services.AnalyticsService.CustomEvent("force_open_chest", parameters);
                }

                var staticData = App.Instance.StaticData;

                long droppedCrystals = 0;
                long droppedArtefacts = 0;
                long cost = 0;

                var simpleChestBreakingTime = staticData.Configs.Burglar.Chests[ChestType.Simple].BreakingTimeInMinutes;
                var royalChestBreakingTime = staticData.Configs.Burglar.Chests[ChestType.Royal].BreakingTimeInMinutes;

                var chestForceCompletePricePer30Minutes =
                    staticData.Configs.Burglar.ChestForceCompletePricePer30Minutes;

                if (string.IsNullOrEmpty(Id))
                {
                    var breakingMinutesLeft = Type == ChestType.Royal
                        ? royalChestBreakingTime
                        : simpleChestBreakingTime;

                    var forceOpenPrice = Math.Ceiling(breakingMinutesLeft / 30f) * chestForceCompletePricePer30Minutes;

                    var res = App.Instance.Player.Burglar.OpenChestForcedInternal(staticData.Configs, Type, (int)forceOpenPrice);

                    OpenResult(res.Item1, res.Item3, res.Item2);
                }


                var chestData = App.Instance.Player.Burglar.Chests.Find(x => x.Id == Id);

                if (chestData == null)
                {
                    Debug.LogError("Cant find chest in player");
                    return;
                }

                var chestType = chestData.Type;
                var userChestStartBreakingTimeData = chestData.StartBreakingTime;

                // force open chest who not started breaking
                if (userChestStartBreakingTimeData == null)
                {
                    var breakingMinutesLeft2 = chestType == ChestType.Royal
                        ? royalChestBreakingTime
                        : simpleChestBreakingTime;

                    var forceOpenPrice2 =
                        Math.Ceiling(breakingMinutesLeft2 / 30f) * chestForceCompletePricePer30Minutes;

                    var res = App.Instance.Player.Burglar.OpenChestForcedInternal(staticData.Configs, chestType, (int)forceOpenPrice2, chestData);

                    OpenResult(res.Item1, res.Item3, res.Item2);
                }
                else
                {
                    var now = TimeManager.Instance.NowUnixSeconds;
                    var minute = 60;
                    var userChestStartBreakingTime = TimeManager.Instance
                        .ToDateTimeOffset((DateTime)userChestStartBreakingTimeData).ToUnixTimeSeconds();
                    var breakedTime = chestType == ChestType.Royal
                        ? userChestStartBreakingTime + (royalChestBreakingTime * minute)
                        : userChestStartBreakingTime + (simpleChestBreakingTime * minute);

                    if (breakedTime <= now)
                    {
                        Debug.LogError("Break time error");
                        return;
                    }

                    var breakingMinutesLeft3 = (breakedTime - now) / (60);
                    var forceOpenPrice3 =
                        Math.Ceiling(breakingMinutesLeft3 / 30f) * chestForceCompletePricePer30Minutes;

                    var res = App.Instance.Player.Burglar.OpenChestForcedInternal(staticData.Configs, chestType, (int)forceOpenPrice3, chestData);
                    OpenResult(res.Item1, res.Item3, res.Item2);
                }
            }
        }

        public void SpeedUp()
            {
                if (!IsBreakingStarted || IsBroken)
                    return;

                App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.SpeedupChestBreaking);
            }


            private void Subscribe()
            {
                EventManager.Instance.Subscribe<UnscaledSecondsTickEvent>(OnSecondTick);
                App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted += OnAdRewardedVideoCompleted;
            }

            private void Unsubscribe()
            {
                if (App.Instance != null)
                    App.Instance.Services.AdvertisementService.OnRewardedVideoCompleted -= OnAdRewardedVideoCompleted;

                if (EventManager.Instance != null)
                    EventManager.Instance.Unsubscribe<UnscaledSecondsTickEvent>(OnSecondTick);
            }

            private void OnAdRewardedVideoCompleted(AdSource adSource, bool isShowed, string rewardId, int rewardAmount)
            {
                if (!isShowed)
                    return;

                if (adSource != AdSource.SpeedupChestBreaking)
                    return;

                var staticData = App.Instance.StaticData;

                var chestBreakingSpeedUpMinutes = staticData.Configs.Burglar.ChestBreakingSpeedUpMinutes;

                Chest chest = null;

                foreach (var burglarChest in App.Instance.Player.Burglar.Chests)
                {
                    if (burglarChest.StartBreakingTime != null)
                    {
                        chest = burglarChest;
                    }

                }

                if (chest?.StartBreakingTime != null)
                {
                    var userBreakingChestStartTime = TimeManager.Instance
                        .ToDateTimeOffset((DateTime)chest.StartBreakingTime).ToUnixTimeSeconds();

                    userBreakingChestStartTime -= chestBreakingSpeedUpMinutes * 60;

                    var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(userBreakingChestStartTime);

                    chest.StartBreakingTime = dateTimeOffset.UtcDateTime;

                    StartBreakingTime =
                        StartBreakingTime.Value.AddMinutes(-App.Instance.StaticData.Configs.Burglar
                            .ChestBreakingSpeedUpMinutes);
                    var parameters = new CustomEventParameters
                        { Int = new Dictionary<string, int> { { "type", (int)Type }, { "level", Level } } };
                    App.Instance.Services.AnalyticsService.CustomEvent("speedup_chest", parameters);
                }

            }

            private void OnSecondTick(UnscaledSecondsTickEvent eventData)
            {
                var timeLeft = StaticHelper.GetChestBreakingTimeLeft(Type, StartBreakingTime.Value);
                var breakingTimeLeftChangeEvent = new ChestBreakingTimeLeftEvent(this, timeLeft);
                EventManager.Instance.Publish(breakingTimeLeftChangeEvent);

                if (!IsBreakingStarted || !IsBroken)
                    return;

                Unsubscribe();
                var chestBrakedEvent = new ChestBreakedEvent(this);
                EventManager.Instance.Publish(chestBrakedEvent);
            }

 
        }

    }
