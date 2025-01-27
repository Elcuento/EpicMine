using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public sealed class TradeAffairsDailyTask : DailyTask
    {
        public string IngotStaticId { get; private set; }


        public TradeAffairsDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.ItemDailyTask dtoTask = null) : base(staticTask, dtoTask)
        {
             Subscribe();

            if (dtoTask != null)
                IngotStaticId = dtoTask.ItemStaticId;
        }


        public override void Subscribe()
        {
            base.Subscribe();

            if (string.IsNullOrEmpty(IngotStaticId))
            {
                var lastOpenedTier = App.Instance.Player.Dungeon.Tiers.LastOrDefault(t => t.IsOpen);
                if (lastOpenedTier == null)
                    return;

                var randomRecipe = App.Instance.StaticData.Recipes.FirstOrDefault(
                    r => r.Ingredient1Id == lastOpenedTier.StaticTier.WallItem2Id
                         && string.IsNullOrEmpty(r.Ingredient2Id)
                         && string.IsNullOrEmpty(r.Ingredient3Id));

                if (randomRecipe == null)
                    return;

                IngotStaticId = randomRecipe.Id;
            }

            if (!IsCompleted)
            {
                EventManager.Instance.Subscribe<InventoryItemChangeEvent>(OnInventoryItemChange);
                EventManager.Instance.Subscribe<InventoryItemRemoveExistEvent>(OnInventoryItemRemove);
            }
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            EventManager.Instance.Unsubscribe<InventoryItemChangeEvent>(OnInventoryItemChange);
            EventManager.Instance.Unsubscribe<InventoryItemRemoveExistEvent>(OnInventoryItemRemove);
        }


        private void OnInventoryItemRemove(InventoryItemRemoveExistEvent existEventData)
        {
            if (existEventData.ItemId == IngotStaticId && existEventData.SpendType == SpendType.Selling)
                Collect(existEventData.Amount);
        }

        private void OnInventoryItemChange(InventoryItemChangeEvent eventData)
        {
            if (eventData.Item.Id == IngotStaticId && !eventData.IsAdded && eventData.SpendType == SpendType.Selling)
                Collect(eventData.DifferenceAmount);
        }
    }
}