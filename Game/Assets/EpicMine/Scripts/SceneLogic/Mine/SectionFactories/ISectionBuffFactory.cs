namespace BlackTemple.EpicMine
{
    public interface ISectionBuffFactory
    {
        MineSceneSectionBuff CreateExplosiveStrikeBuff();

        MineSceneSectionBuff CreateFreezingBuff();

        MineSceneSectionBuff CreateAcidBuff();

        MineSceneSectionBuff CreateTntBuff();
    }
}