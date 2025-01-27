using BlackTemple.EpicMine;

public class VillageSceneMerchant : VillageSceneCharacter
{
    protected override void Start()
    {
        _controller.RegisterVillageQuestCharacter(this, _questArrow);

        CheckQuest();
    }
}
