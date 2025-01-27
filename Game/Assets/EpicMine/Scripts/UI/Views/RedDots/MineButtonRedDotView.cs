using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public class MineButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            EventManager.Instance.Subscribe<ArtefactsAmountChangeEvent>(OnArtefactsChange);
            EventManager.Instance.Subscribe<MineCompleteEvent>(OnMineComplete);
            EventManager.Instance.Subscribe<TierOpenEvent>(OnTierOpen);
        }

        private void Start()
        {
            Calculate();
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<ArtefactsAmountChangeEvent>(OnArtefactsChange);
                EventManager.Instance.Unsubscribe<MineCompleteEvent>(OnMineComplete);
                EventManager.Instance.Unsubscribe<TierOpenEvent>(OnTierOpen);
            }
        }


        private void OnArtefactsChange(ArtefactsAmountChangeEvent eventData)
        {
            Calculate();
        }

        private void OnMineComplete(MineCompleteEvent eventData)
        {
            if (!eventData.Mine.IsLast)
                return;

            Calculate();
        }

        private void OnTierOpen(TierOpenEvent eventData)
        {
            Hide();
        }

        private void Calculate()
        {
            Hide();

            var artefacts = MineHelper.GetTierRequireArtefacts(App.Instance.Player.Dungeon.LastOpenedTier.Number + 1);
            if (App.Instance.Player.Artefacts.Amount < artefacts)
                return;

            if (!App.Instance.Player.Dungeon.LastOpenedTier.IsComplete)
                return;

            Show();
        }
    }
}