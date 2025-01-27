using System;

namespace BlackTemple.EpicMine.Dto
{
    public class DailyTask
    {
        public string Id;

        public DateTime CreationDate;

        public int CollectedAmount;

        public bool IsRewardTaken;

        public DailyTask(string id, DateTime creationDate, int collectedAmount, bool isRewardTaken)
        {
            Id = id;
            CreationDate = creationDate;
            CollectedAmount = collectedAmount;
            IsRewardTaken = isRewardTaken;
        }
    }
}