using System;
using Newtonsoft.Json;

namespace BlackTemple.EpicMine.Dto
{
    public class ItemDailyTask : DailyTask
    {
        [JsonProperty("ItemStaticId")]
        public string ItemStaticId { get; private set; }

        public ItemDailyTask(string id, DateTime creationDate, int collectedAmount, bool isRewardTaken, string itemId) : base(id, creationDate, collectedAmount, isRewardTaken)
        {
            ItemStaticId = itemId;
        }
    }
}