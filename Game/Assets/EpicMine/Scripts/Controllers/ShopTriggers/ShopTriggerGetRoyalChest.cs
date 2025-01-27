using BlackTemple.Common;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;
using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public class ShopTriggerGetRoyalChest : ShopTrigger
    {
        public ChestType RequireChestOpen;
        public ChestType ChestOpen;

        public ShopTriggerGetRoyalChest(ChestType type, string offerId, bool isCompleted = false) 
            : base(offerId, isCompleted)
        {
            RequireChestOpen = type;
        }

        public override void OnStart()
        {
            EventManager.Instance.Subscribe<OpenChestEvent>(OnOpenChest);
        }

        public override void OnCompleted()
        {
            EventManager.Instance.Unsubscribe<OpenChestEvent>(OnOpenChest);
        }

        public override void OnReset()
        {
            EventManager.Instance.Unsubscribe<OpenChestEvent>(OnOpenChest);
        }

        public override void OnCheck()
        {
            if (ChestOpen == RequireChestOpen)
            {
                SetCompleted();
            }
        }


        public void OnOpenChest(OpenChestEvent data)
        {
            ChestOpen = data.Type;
            SetCheck();
        }
    }
}
