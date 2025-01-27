namespace BlackTemple.EpicMine
{
    public interface ISectionFactory
    {
        MineSceneSection CreateWallSection();

        MineSceneSection CreateChestSection();

        MineSceneSection CreateEnchantedChestSection();

        MineSceneSection CreateEmptySection();

        MineSceneSection CreateWallOrChestSection(bool allowEnchantedChests = true);

        MineSceneSection CreateEmptyOrChestSection(bool allowedEnchantedChests = true);

        MineSceneSection CreateBlacksmithSection();

        MineSceneSection CreateBossSection();

        MineSceneSection CreateMonsterOrNullSection();

        MineSceneSection CreateMonsterSection(string id);

        MineSceneSection CreateGodSection();

        MineSceneSection CreateDoorSection();

        MineSceneSection CreateLastDoorSection();
    }
}