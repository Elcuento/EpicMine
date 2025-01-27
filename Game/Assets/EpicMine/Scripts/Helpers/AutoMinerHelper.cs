using System.Linq;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public static class AutoMinerHelper
    {
        public static GameObject GetModel(Transform transform)
        {
            var prefab = Resources.Load($"{Paths.ResourcesPrefabsAutoMinersPath}autoMiner{GetLevel()}");
            return Object.Instantiate(prefab, transform, false) as GameObject;
        }

        public static int GetLevel()
        {
            return App.Instance.Player.AutoMiner.SpeedLevel.StaticLevel.Stage - 1;
        }

        public static float GetLevelFillProgress()
        {
            var currentLevel = GetLevel();
            
            var nextLevel = App.Instance.StaticData.AutoMinerSpeedLevels.Find(x => x.Stage - 1 > currentLevel);

            if (nextLevel == null)
                return 1;

            var allUpdates = App.Instance.StaticData.AutoMinerSpeedLevels.Where(x => x.Stage - 1 == currentLevel).ToList();

            var currentPosition = allUpdates.IndexOf(App.Instance.Player.AutoMiner.SpeedLevel.StaticLevel);

            return currentPosition / (float) allUpdates.Count;
        }

        public static int GetLevelNextValueProgress()
        {
            var currentLevel = GetLevel();

            return App.Instance.StaticData.AutoMinerSpeedLevels.Count(x => x.Stage - 1 == currentLevel);
        }

        public static int GetLevelCurrentValueProgress()
        {
            var currentLevel = GetLevel();

            var allUpdates = App.Instance.StaticData.AutoMinerSpeedLevels.Where(x => x.Stage - 1== currentLevel).ToList();

            return allUpdates.IndexOf(App.Instance.Player.AutoMiner.SpeedLevel.StaticLevel);
      
        }
    }
}
