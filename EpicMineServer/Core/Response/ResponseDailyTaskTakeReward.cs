using System;
using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseDailyTaskTakeReward : Response<RequestDataDailyTaskTakeReward>
    {

        public ResponseDailyTaskTakeReward(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {

                var staticData = Peer.GetStaticData();

                var staticDailyTaskData = staticData.DailyTasks.Find(x => x.Id == Value.Id);

                if (staticDailyTaskData == null)
                {
                    Log("Task not exist " + Value?.Id);
                    return false;
                }

                var userDailyTasksData = Peer.Player.Data.DailyTasks.TodayTaken;
              /*  var today = DateTime.UtcNow;


                var canTakenReward = true;
                var todayTakenRewardCount = 0;

                for (var index = 0; index < userDailyTasksData.Count; index++)
                {
                    var dailyTask = userDailyTasksData[index];

                    var date = AMTServerDLL.Utils.FromUnix(dailyTask.TakenTime);
                    if (date.DayOfYear != today.DayOfYear)
                    {
                        userDailyTasksData.Remove(userDailyTasksData[index]);
                        index--;
                    }
                    else
                    {
                        if (dailyTask.Id == Value.Id)
                        {
                            canTakenReward = false;
                            Log("Task already taken");
                        }

                        todayTakenRewardCount++;
                    }
                }

                if (todayTakenRewardCount >= staticData.Configs.DailyTasks.MaxCount)
                {
                    Log("Task already maximum");
                    canTakenReward = false;
                }


                if (!canTakenReward)
                    return false;*/

                Peer.AddCurrency(CurrencyType.Crystals, staticDailyTaskData.RewardAmount);

                var task = userDailyTasksData.Find(x => x.Id == Value.Id);

                if (task == null)
                {
                    task = new CommonDLL.Dto.DailyTask {Id = Value.Id};
                    userDailyTasksData.Add(task);
                }

                task.TakenTime = AMTServerDLL.Utils.GetUnixTime();

                Peer.SavePlayer();

                return true;
            }
        }
    }
}