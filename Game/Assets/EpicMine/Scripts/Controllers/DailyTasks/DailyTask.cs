using System;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public class DailyTask
    {
        public CommonDLL.Static.DailyTask StaticTask { get; }
        public DateTime CreationDate { get; }
        public int CollectedAmount { get; private set; }
        public bool IsCompleted => CollectedAmount >= StaticTask.RequiredAmount;
        public bool IsRewardTaken { get; private set; }


        public DailyTask(CommonDLL.Static.DailyTask staticTask, Dto.DailyTask dtoTask = null)
        {
            StaticTask = staticTask;

            if (dtoTask != null)
            {
                CreationDate = dtoTask.CreationDate;
                CollectedAmount = dtoTask.CollectedAmount;
                IsRewardTaken = dtoTask.IsRewardTaken;
            }
            else
                CreationDate = DateTime.UtcNow.Date;
        }

        public void TakeReward()
        {
            if (!IsCompleted || IsRewardTaken)
                return;

            var reward = new Currency(CurrencyType.Crystals, StaticTask.RewardAmount);
            App.Instance.Player.Wallet.Add(reward, IncomeSourceType.FromDailyTask);
            IsRewardTaken = true;

            var takeEvent = new DailyTaskTakeEvent(this);
            EventManager.Instance.Publish(takeEvent);

            var customEventParameters = new CustomEventParameters
            {
                String = new Dictionary<string, string>
                {
                    { "id", StaticTask.Id }
                },
                Int = new Dictionary<string, int>
                {
                    { "reward_amount", StaticTask.RewardAmount }
                }
            };
            App.Instance.Services.AnalyticsService.CustomEvent("take_daily_task_reward", customEventParameters);
        }

        public virtual void Subscribe() { }
        public virtual void Unsubscribe() { }


        protected void Collect(int amount = 1)
        {
            CollectedAmount += amount;

            if (IsCompleted)
                OnCollectComplete();
        }


        protected virtual void OnCollectComplete()
        {
            var completeEvent = new DailyTaskCompleteEvent(this);
            EventManager.Instance.Publish(completeEvent);

            Unsubscribe();
        }
    }
}