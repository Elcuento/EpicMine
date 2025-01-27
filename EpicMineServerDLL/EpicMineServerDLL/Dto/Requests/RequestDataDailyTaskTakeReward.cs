namespace AMTServerDLL.Dto
{
    public class RequestDataDailyTaskTakeReward : SendData
    {
        public string Id;

        public RequestDataDailyTaskTakeReward(string id)
        {
            Id = id;
        }
    }
}