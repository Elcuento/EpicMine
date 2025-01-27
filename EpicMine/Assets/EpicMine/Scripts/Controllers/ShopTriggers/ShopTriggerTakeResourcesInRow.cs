using BlackTemple.Common;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public class ShopTriggerTakeResourcesInRow : ShopTrigger
    {
        public int RequireRowCount;
        public int CurrentRowCount;

        public ShopTriggerTakeResourcesInRow(int requireRowCount, string offerId, bool isCompleted = false) 
            : base(offerId, isCompleted)
        {
            RequireRowCount = requireRowCount;
        }

        public override void OnStart()
        {
            EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnAddItem);
        }

        public override void OnReset()
        {
            EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnAddItem);
        }

        public override void OnCompleted()
        {
            EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnAddItem);
        }

        public override void OnCheck()
        {
            if (CurrentRowCount >= RequireRowCount)
            {
                SetCompleted();
            }
        }

        public void OnAddItem(InventoryItemAddEvent eventData)
        {
            if (eventData.IncomeSourceType == IncomeSourceType.FromCraft)
            {
                CurrentRowCount = eventData.Item.Amount;
                SetCheck();
            }
        }
    }
}
