using BlackTemple.Common;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;

namespace BlackTemple.EpicMine
{
    public class ShopTriggerBuyWorkshopSlot : ShopTrigger
    {
        public int RequiredSlots;

        public ShopTriggerBuyWorkshopSlot(int slot, string offerId, bool isCompleted = false) : base(offerId, isCompleted)
        {
            RequiredSlots = slot;
        }

        public override void OnStart()
        {
            EventManager.Instance.Subscribe<WorkshopSlotUnlockEvent>(OnOpenSlot);
        }

        public override void OnCompleted()
        {
            EventManager.Instance.Unsubscribe<WorkshopSlotUnlockEvent>(OnOpenSlot);
        }

        public override void OnReset()
        {
            EventManager.Instance.Unsubscribe<WorkshopSlotUnlockEvent>(OnOpenSlot);
        }

        public override void OnCheck()
        {
            if (App.Instance.Player.Workshop.Slots.FindAll(x=>x.IsUnlocked).Count >= RequiredSlots 
               || App.Instance.Player.Workshop.SlotsShard.FindAll(x => x.IsUnlocked).Count >= RequiredSlots)
            {
                SetCompleted();
            }
        }

        public void OnOpenSlot(WorkshopSlotUnlockEvent data)
        {
            SetCheck();
        }
    }
}
