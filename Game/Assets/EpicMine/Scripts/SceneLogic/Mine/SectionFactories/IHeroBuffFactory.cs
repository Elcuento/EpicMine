namespace BlackTemple.EpicMine
{
    public interface IHeroBuffFactory
    {
        MineSceneHeroBuff CreateDamagePotionBuff();

        MineSceneHeroBuff CreateAcidBuff();

        MineSceneHeroBuff CreatePrestigeBuff();
    }
}