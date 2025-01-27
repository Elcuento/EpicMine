
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class PvpSectionFactory : ISectionFactory
    {
        private readonly Transform _container;

        private readonly int _arenaNumber;


        public PvpSectionFactory(Transform container)
        {
            _container = container;

            var matchData = PvpArenaNetworkController.GetMatchData();
            if (matchData != null)
                _arenaNumber = matchData.Arena;
            else Debug.LogError("No arena data");
        }


        public MineSceneSection CreateWallSection()
        {
            return CreateSection("PvpWall");
        }

        public MineSceneSection CreateChestSection()
        {
            return CreateSection("PvpChest");
        }

        public MineSceneSection CreateEnchantedChestSection()
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateEmptySection()
        {
            return CreateSection("Empty");
        }

        public MineSceneSection CreateWallOrChestSection(bool allowEnchantedChests = true)
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateEmptyOrChestSection(bool allowEnchantedChests = true)
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateBlacksmithSection()
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateBossSection()
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateMonsterOrNullSection()
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateMonsterSection(string id)
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateGhostSection(Ghost ghost)
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateGodSection()
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateDoorSection()
        {
            Debug.LogError("No use");
            return null;
        }

        public MineSceneSection CreateLastDoorSection()
        {
            Debug.LogError("No use");
            return null;
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
            var leagueEnvironmentName = $"league_{ _arenaNumber + 1 }_environment";
            var leagueEnvironmentPrefab = Resources.Load<GameObject>($"{Paths.ResourcesPrefabsEnvironmentsPath}{leagueEnvironmentName}");
            Object.Instantiate(leagueEnvironmentPrefab, section, false);
        }
    }
}