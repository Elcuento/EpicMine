using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;

public class ShopTriggerDiedOnBoss : ShopTrigger
{
    public int RequiredDieInRow;
    public int Died;

    public ShopTriggerDiedOnBoss(int die, string offerId, bool isCompleted = false) : base(offerId, isCompleted)
    {
        RequiredDieInRow = die;
    }

    public override void OnStart()
    {
        EventManager.Instance.Subscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeDestroy);
        EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
    }

    public override void OnCompleted()
    {
        EventManager.Instance.Unsubscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeDestroy);
        EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
    }

    public override void OnReset()
    {
        EventManager.Instance.Unsubscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeDestroy);
        EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
    }

    public override void OnCheck()
    {
        if (Died >= RequiredDieInRow)
        {
            SetCompleted();
        }
    }

    public void OnSectionPassed(MineSceneSectionPassedEvent data)
    {
        var bossWall = data.Section as MineSceneBossSection;

        if (bossWall != null)
            Died = 0;
    }

    public void OnPickaxeDestroy(MineScenePickaxeDestroyedEvent destroy)
    {
        if (SceneManager.Instance.CurrentScene == ScenesNames.Mine)
        {
            var bossWall = destroy.Section as MineSceneBossSection;

            if (bossWall != null)
                Died++;
        }

        SetCheck();
    }
}
