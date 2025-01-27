using BlackTemple.Common;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;

namespace BlackTemple.EpicMine
{
    public class ShopTriggerBossKill : ShopTrigger
    {
        public int RequireBossKilled;
        public int BossKilled;

        public ShopTriggerBossKill(int bossKill, string offerId, bool isCompleted = false) : base(offerId, isCompleted)
        {
            RequireBossKilled = bossKill;
            var lastTier = App.Instance.Player.Dungeon.LastOpenedTier;
            BossKilled = lastTier?.Number ?? 0;
        }

        public override void OnStart()
        {
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnBossKilled);
        }

        public void OnBossKilled(MineSceneSectionPassedEvent tier)
        {
            if(tier.Section is MineSceneBossSection)
            {
                var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

                BossKilled = selectedTier.Number+1;
            }

            SetCheck();
        }

        public override void OnReset()
        {
            EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnBossKilled);
        }

        public override void OnCompleted()
        {
            EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnBossKilled);
        }

        public override void OnCheck()
        {
            if (BossKilled >= RequireBossKilled)
            {
                SetCompleted();
            }
        }
    }
}
