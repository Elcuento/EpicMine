
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class DefaultSectionFactory : ISectionFactory
    {
        private readonly Transform _container;

        private readonly int _selectedTierNumber;

        private readonly int _selectedMineNumber;


        public DefaultSectionFactory(Transform container)
        {
            _container = container;

            _selectedTierNumber = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier).Number;

            _selectedMineNumber = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine).Number;

        }

        public MineSceneSection CreateWallSection()
        {
            return CreateSection("Wall");
        }

        public MineSceneSection CreateChestSection()
        {
            return CreateSection("Chest");
        }

        public MineSceneSection CreateEnchantedChestSection()
        {
            return CreateSection("EnchantedChest");
        }

        public MineSceneSection CreateEmptySection()
        {
            return CreateSection("Empty");
        }

        public MineSceneSection CreateWallOrChestSection(bool allowEnchantedChests = true)
        {
            var randomChestValue = MineHelper.GetRandomChestSectionValue(_selectedTierNumber, _selectedMineNumber);
            if (randomChestValue)
            {
                if (allowEnchantedChests)
                {
                    var randomPercent = Random.Range(0, 100f);
                    if (randomPercent < App.Instance.StaticData.Configs.EnchantedChests.Chance)
                        return CreateSection("EnchantedChest");
                }

                return CreateSection("Chest");
            }

            return CreateSection("Wall");
        }

        public MineSceneSection CreateEmptyOrChestSection(bool allowEnchantedChests = true)
        {
            var randomChestValue = MineHelper.GetRandomChestSectionValue(_selectedTierNumber, _selectedMineNumber);
            if (randomChestValue)
            {
                if (allowEnchantedChests)
                {
                    var randomPercent = Random.Range(0, 100f);
                    if (randomPercent < App.Instance.StaticData.Configs.EnchantedChests.Chance)
                        return CreateSection("EnchantedChest");
                }

                return CreateSection("Chest");
            }

            return CreateSection("Empty");
        }

        public MineSceneSection CreateBlacksmithSection()
        {
            return CreateSection("Blacksmith");
        }

        public MineSceneSection CreateBossSection()
        {
            return CreateSection("Boss");
        }

        public MineSceneSection CreateMonsterOrNullSection()
        {
            var monsterSpawnData = StaticHelper.GetMonsterSpawn(_selectedTierNumber, _selectedMineNumber);
            var random = Random.Range(0, 100);

            foreach (var i in monsterSpawnData.Chance)
            {
                 if (i.Value >= random)
                   return CreateMonsterSection("Monster", i.Key);
            }

            return null;
        }

        public MineSceneSection CreateMonsterSection(string id)
        {
            return CreateMonsterSection("Monster", id);
        }

        public MineSceneSection CreateGodSection()
        {
            return CreateSection("God");
        }

        public MineSceneSection CreateDoorSection()
        {
            return CreateSection("Door");
        }

        public MineSceneSection CreateLastDoorSection()
        {
            return CreateSection("LastDoor");
        }

        private MineSceneMonsterSection CreateMonsterSection(string path, string monsterId)
        {
            var prefab = Resources.Load<MineSceneMonsterSection>($"{Paths.ResourcesPrefabsSectionsPath}{path}");
            var section = Object.Instantiate(prefab, _container, false);
            section.SetMonsterId(monsterId);
            CreateEnvironment(section.transform);
            return section;
        }

        private MineSceneSection CreateSection(string path)
        {
            var prefab = Resources.Load<MineSceneSection>($"{Paths.ResourcesPrefabsSectionsPath}{path}");
            var section = Object.Instantiate(prefab, _container, false);
            CreateEnvironment(section.transform);
            return section;
        }

        private void CreateEnvironment(Transform section)
        {
            var tierEnvironmentName = $"tier_{_selectedTierNumber + 1}_environment";

            var tierEnvironmentPrefab = Resources.Load<GameObject>($"{Paths.ResourcesPrefabsEnvironmentsPath}{tierEnvironmentName}");
            Object.Instantiate(tierEnvironmentPrefab, section, false);
        }
    }
}