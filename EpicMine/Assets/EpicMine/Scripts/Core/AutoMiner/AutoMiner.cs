using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;
using FeaturesType = CommonDLL.Static.FeaturesType;
using Random = UnityEngine.Random;


namespace BlackTemple.EpicMine.Core
{
    public class AutoMiner
    {
        public AutoMinerSpeedLevel SpeedLevel { get; private set; }

        public AutoMinerCapacityLevel CapacityLevel { get; private set; }

        public bool IsOpen { get; private set; }

        public int Level { get; private set; }

        public int Tier { get; private set; }

        public long TimeEnd { get; private set; }

        public long TimeStart { get; private set; }

        public bool Started { get; private set; }

        public Dictionary<string, int> EarnedResources { get; private set; }

        public long TimeLeft => TimeEnd - TimeManager.Instance.NowUnixSeconds;

        public long TimeLeftTotal => (CapacityLevel.StaticLevel.Capacity - Capacity) * Cost;

        public bool IsFull => Capacity >= CapacityLevel.StaticLevel.Capacity;

        public int Capacity => EarnedResources.Sum(x => x.Value);

        public int Cost => AutoMinerLocalConfig.HarvestPeriod / SpeedLevel.StaticLevel.Amount;

        private bool _isWork;

        private List<int> _chances;

        public AutoMiner(CommonDLL.Dto.AutoMiner autoMiner)
        {
            EarnedResources = new Dictionary<string, int>();
            SpeedLevel = new AutoMinerSpeedLevel(autoMiner?.SpeedLevel?.Number ?? 0);
            CapacityLevel = new AutoMinerCapacityLevel(autoMiner?.CapacityLevel?.Number ?? 0);

            if (autoMiner != null)
            {
                if (autoMiner.Items != null)
                {
                    foreach (var dataEarnedResource in autoMiner.Items)
                    {
                        if (EarnedResources.ContainsKey(dataEarnedResource.Key))
                        {
                            EarnedResources[dataEarnedResource.Key] += dataEarnedResource.Value;
                        }
                        else
                        {
                            EarnedResources.Add(dataEarnedResource.Key, dataEarnedResource.Value);
                        }
                    }
                }

                TimeStart = autoMiner.TimerStart;
                TimeEnd = autoMiner.TimerEnd;
                Tier = autoMiner.Tier;
                Started = autoMiner.Started;
            }

            if (!Started || !IsOpen)
                return;

            Recalculate();

            
            EventManager.Instance.Subscribe<AutoMinerChangeSpeedLevelEvent>(OnUpdateLevel);
            EventManager.Instance.Subscribe<AutoMinerChangeCapacityLevelEvent>(OnUpdateCapacity);
        }

        public void Initialize()
        {
            IsOpen = App.Instance.Player.Features.Exist(FeaturesType.AutoMiner);
            Level = AutoMinerHelper.GetLevel();
            _isWork = IsOpen && Started && (!IsFull);
            _chances = GetItemDropChance();


            if (!IsOpen)
            {
                EventManager.Instance.Subscribe<AddNewFeatureEvent>(OnAddNewFeature);
            }

            if (_isWork)
                ContinueAutoMiner();
        }

        public List<int> GetItemDropChance()
        {
            var dic = new List<int>();

            var tier = App.Instance.Player.Dungeon.Tiers[Tier];

            var mine = tier.Mines.Count > 2 ? tier.Mines[2] : tier.Mines.First();

            var staticMine = StaticHelper.GetMine(tier.Number, mine.Number);
            var mineCommonSettings = StaticHelper.GetMineCommonSettings(tier.Number, mine.Number);

            var firstItemChance = staticMine.Item1Percent ?? mineCommonSettings.Item1Percent;
            var secondItemChance = staticMine.Item2Percent ?? mineCommonSettings.Item2Percent;
            var thirdItemChance = staticMine.Item3Percent ?? mineCommonSettings.Item3Percent;

            // exclude chest percent
            var itemsChanceSum = firstItemChance + secondItemChance + thirdItemChance;
            dic.Add(Mathf.RoundToInt(firstItemChance / itemsChanceSum * 100));
            dic.Add(Mathf.RoundToInt(secondItemChance / itemsChanceSum * 100));
            dic.Add(Mathf.RoundToInt(thirdItemChance / itemsChanceSum * 100));
            return dic;
        }

        private void OnUpdateCapacity(AutoMinerChangeCapacityLevelEvent eventData)
        {
            if (!_isWork && Started)
            {
                StartAutoMiner();
            }

        }

        private void OnAddNewFeature(AddNewFeatureEvent eventData)
        {
            Open();
        }

        public void OnUpdateLevel(AutoMinerChangeSpeedLevelEvent eventData)
        {
            TimeStart = 0;
            TimeEnd = 0;

            Recalculate();

            var newLevel = AutoMinerHelper.GetLevel();

           if (newLevel <= Level)
               return;
           
               Level = newLevel;

               EventManager.Instance.Publish(new AutoMinerChangeMinerLevelEvent(Level));
               WindowManager.Instance.Show<WindowAutoMinerLevelUp>();
           
        }


        private void Recalculate()
        {
            if (IsFull)
                return;

            if (TimeStart <= 0)
            {
                TimeStart = TimeManager.Instance.NowUnixSeconds;
                TimeEnd = TimeManager.Instance.NowUnixSeconds + Cost;
                return;
            }

            var totalHarvested = 0;

            if (TimeManager.Instance.NowUnixSeconds >= TimeEnd)
            {
                totalHarvested = 1;

                var timeLeft = TimeManager.Instance.NowUnixSeconds - TimeEnd;

                while (timeLeft > Cost && (Capacity + totalHarvested) < CapacityLevel.StaticLevel.Capacity)
                {
                    timeLeft -= Cost;
                    totalHarvested++;
                }
            }

            Harvest(totalHarvested);

             if(totalHarvested >= CapacityLevel.StaticLevel.Capacity)
                 StopAutoMiner();
        }

        public void ContinueAutoMiner()
        {
            EventManager.Instance.Subscribe<SecondsTickEvent>(OnTick);
            EventManager.Instance.Publish(new AutoMinerStartEvent());
            _isWork = true;
        }

        public void StartAutoMiner(int tier)
        {
            StopAutoMiner();

            Started = true;
            Tier = tier;
            TimeStart = TimeManager.Instance.NowUnixSeconds;
            TimeEnd = TimeStart + Cost;
            _isWork = true;
            _chances = GetItemDropChance();

            EventManager.Instance.Subscribe<SecondsTickEvent>(OnTick);

            EventManager.Instance.Publish(new AutoMinerChangeEvent());
            EventManager.Instance.Publish(new AutoMinerStartEvent());
        }

        public void StartAutoMiner()
        {
            Debug.Log("ASD");
            StopAutoMiner();

            Started = true;
            TimeStart = TimeManager.Instance.NowUnixSeconds;
            TimeEnd = TimeStart + Cost;
            _isWork = true;

            EventManager.Instance.Subscribe<SecondsTickEvent>(OnTick);

            EventManager.Instance.Publish(new AutoMinerChangeEvent());
            EventManager.Instance.Publish(new AutoMinerStartEvent());
        }

        public void StopAutoMiner()
        {
            Debug.Log("Stop");
            EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTick);
            EventManager.Instance.Publish(new AutoMinerEndEvent());
            _isWork = false;

            if (IsFull)
                EventManager.Instance.Publish(new AutoMinerFullEvent());
        }

        private void Harvest(int earned)
        {
            if (earned <= 0)
                return;


            var tier = App.Instance.Player.Dungeon.Tiers[Tier];

            if (tier != null)
            {
                var resourceAdded = 0;

                var resourceAdd = new Dictionary<string, int>();

                var droppedItems = new List<string>
                    {tier.StaticTier.WallItem1Id, tier.StaticTier.WallItem2Id, tier.StaticTier.WallItem3Id};

                var outTime = 200;

                while (resourceAdded < earned)
                {
                    outTime--;

                    if (outTime <= 0)
                    {
                        App.Instance.Services.LogService.LogError("Wrong random data on items in miner");
                        break;
                    }

                    var items = tier.UnlockedDropItems.Where(x => !string.IsNullOrEmpty(x)).ToList();

                    if (items.Count <= 0)
                        break;

                    for (var index = 0; index < items.Count; index++)
                    {
                        var item1 = items[index];
                        var itemRandomIndex = droppedItems.IndexOf(item1);

                        if (itemRandomIndex == -1)
                            continue;

                        var random = _chances.Count > itemRandomIndex ? _chances[itemRandomIndex] : 0;
                        if (random > Random.Range(0, 100))
                        {
                            if (resourceAdd.ContainsKey(item1))
                                resourceAdd[item1] += 1;
                            else resourceAdd.Add(item1, 1);

                            resourceAdded++;
                        }
                    }
                }

                foreach (var item in resourceAdd)
                {
                    if (EarnedResources.ContainsKey(item.Key))
                    {
                        EarnedResources[item.Key] += item.Value;
                    }else EarnedResources.Add(item.Key, item.Value);
                }
            }

            TimeStart = TimeManager.Instance.NowUnixSeconds;
            TimeEnd = TimeStart + Cost;

            EventManager.Instance.Publish(new AutoMinerChangeEvent());
        }

        private void CollectMiner(Action<int> onCollected, bool isDoubled)
        {

            var staticData = App.Instance.StaticData;

            var autoMinerLevelData = App.Instance.Player.AutoMiner.CapacityLevel;

            var capacity = 0;

            if (autoMinerLevelData != null)
            {
                var autoMinerLevelVal = autoMinerLevelData.Number;
                capacity = staticData.AutoMinerCapacityLevels[autoMinerLevelVal].Capacity;
            }
            else
            {
                capacity = staticData.AutoMinerCapacityLevels[0].Capacity;
            }

            if (capacity > Capacity)
                capacity = Capacity;

            var isFull = IsFull;

            capacity *= (isDoubled ? 2 : 1);

            EarnedResources.Clear();

            if (isFull)
                StartAutoMiner();

            EventManager.Instance.Publish(new AutoMinerChangeEvent());

            onCollected?.Invoke(capacity);
        }
        public void Collect(Action<int> onCollected, bool isDoubled)
        {
            if (EarnedResources.Count <= 0)
                return;

            CollectMiner(onCollected, isDoubled);
        }

        public void OnTick(SecondsTickEvent data)
        {
            if (TimeManager.Instance.NowUnixSeconds < TimeEnd)
            {
                return;
            }

            var totalHarvested = 1;

            var timeLeft = TimeManager.Instance.NowUnixSeconds - TimeEnd;

            while (timeLeft > Cost && (Capacity + totalHarvested) < CapacityLevel.StaticLevel.Capacity)
            {
                timeLeft -= Cost;
                totalHarvested++;
            }

            Harvest(totalHarvested);

            if (IsFull)
                StopAutoMiner();
        }

        public void Open()
        {
            if (!App.Instance.Player.Features.Exist(CommonDLL.Static.FeaturesType.AutoMiner))
                return;

            IsOpen = true;
            EventManager.Instance.Publish(new AutoMinerOpenEvent());
            WindowManager.Instance.Show<WindowAutoMinerLevelUp>();
        }

        public void Reset()
        {
            EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTick);
            EarnedResources.Clear();
            Started = false;
            _isWork = false;
            IsOpen = false;
            Level = 0;
            Tier = 0;
            _chances?.Clear();
            TimeEnd = 0;
            TimeStart = 0;
        }
    }
}