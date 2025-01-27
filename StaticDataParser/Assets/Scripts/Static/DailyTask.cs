namespace BlackTemple.EpicMine.Static
{
    public class DailyTask
    {
        public string Id { get; }

        public DailyTaskType Type { get; }

        public int Category { get; }

        public int RequiredAmount { get; }

        public int RewardAmount { get; }

        public bool Important { get; }

        public DailyTask(string id, DailyTaskType type, int category, int requiredAmount, int rewardAmount, bool important)
        {
            Id = id.ToLower();
            Type = type;
            Category = category;
            RequiredAmount = requiredAmount;
            RewardAmount = rewardAmount;
            Important = important;
        }
    }
}