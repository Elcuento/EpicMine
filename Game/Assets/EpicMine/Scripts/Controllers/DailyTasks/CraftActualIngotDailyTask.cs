using System.Linq;
using BlackTemple.Common;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public sealed class CraftActualIngotDailyTask : DailyTask
    {
        public string IngotStaticId { get; private set; }


        public CraftActualIngotDailyTask(CommonDLL.Static.DailyTask staticTask, Dto.ItemDailyTask dtoTask = null) : base(staticTask, dtoTask)
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

                var randomOreIndex = Random.Range(1, 4);
                var randomOre = string.Empty;
                switch (randomOreIndex)
                {
                    case 1:
                        randomOre = lastOpenedTier.StaticTier.WallItem1Id;
                        break;
                    case 2:
                        randomOre = lastOpenedTier.StaticTier.WallItem2Id;
                        break;
                    case 3:
                        randomOre = lastOpenedTier.StaticTier.WallItem3Id;
                        break;
                }

                var randomRecipe = App.Instance.StaticData.Recipes.FirstOrDefault(r => r.Ingredient1Id == randomOre);
                if (randomRecipe == null)
                    return;

                IngotStaticId = randomRecipe.Id;
            }

            if (!IsCompleted)
                EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
        }


        private void OnItemAdd(InventoryItemAddEvent eventData)
        {
            if (eventData.Item.Id == IngotStaticId && eventData.IncomeSourceType == CommonDLL.Static.IncomeSourceType.FromCraft)
                Collect(eventData.Item.Amount);
        }
    }
}