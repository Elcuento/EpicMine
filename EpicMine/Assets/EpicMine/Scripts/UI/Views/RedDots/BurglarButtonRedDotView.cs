using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public class BurglarButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            EventManager.Instance.Subscribe<ChestStartBreakingEvent>(OnChestStartBreaking);
            EventManager.Instance.Subscribe<ChestBreakedEvent>(OnChestBroken);
            EventManager.Instance.Subscribe<BurglarChestOpenedEvent>(OnBurglarChestOpened);
        }

        private void Start()
        {
            Calculate();
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<ChestStartBreakingEvent>(OnChestStartBreaking);
                EventManager.Instance.Unsubscribe<ChestBreakedEvent>(OnChestBroken);
                EventManager.Instance.Unsubscribe<BurglarChestOpenedEvent>(OnBurglarChestOpened);
            }
        }

        private void OnChestStartBreaking(ChestStartBreakingEvent eventData)
        {
            Hide();
        }

        private void OnChestBroken(ChestBreakedEvent eventData)
        {
            Show();
        }

        private void OnBurglarChestOpened(BurglarChestOpenedEvent eventData)
        {
            Calculate();
        }

        private void Calculate()
        {
            Hide();

            var isAnyChestBroken = false;
            var isAnyChestStartBreaking = false;

            foreach (var chest in App.Instance.Player.Burglar.Chests)
            {
                if (chest.IsBroken)
                {
                    isAnyChestBroken = true;
                    break;
                }

                if (chest.IsBreakingStarted)
                {
                    isAnyChestStartBreaking = true;
                    break;
                }
            }

            if (isAnyChestBroken || (App.Instance.Player.Burglar.Chests.Count > 0 && !isAnyChestStartBreaking))
                Show();
        }
    }
}