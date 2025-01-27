using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public sealed class ActualOreMiningDailyTask : DailyTask
    {
        public string OreStaticId { get; private set; }


        public ActualOreMiningDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.ItemDailyTask dtoTask = null) : base(staticTask, dtoTask)
        {
            Subscribe();

            if (dtoTask != null)
                OreStaticId = dtoTask.ItemStaticId;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
        }


        public override void Subscribe()
        {
            base.Subscribe();

            if (string.IsNullOrEmpty(OreStaticId))
            {
                var lastOpenedTier = App.Instance.Player.Dungeon.Tiers.LastOrDefault(t => t.IsOpen);
                if (lastOpenedTier == null)
                    return;

                var randomOre = Random.Range(1, 4);
                switch (randomOre)
                {
                    case 1:
                        OreStaticId = lastOpenedTier.StaticTier.WallItem1Id;
                        break;
                    case 2:
                        OreStaticId = lastOpenedTier.StaticTier.WallItem2Id;
                        break;
                    case 3:
                        OreStaticId = lastOpenedTier.StaticTier.WallItem3Id;
                        break;
                }
            }

            if (!IsCompleted)
                EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
        }

        private void OnItemAdd(InventoryItemAddEvent eventData)
        {
            if (eventData.Item.Id == OreStaticId && eventData.IncomeSourceType == CommonDLL.Static.IncomeSourceType.FromMining)
                Collect(eventData.Item.Amount);
        }
    }
}