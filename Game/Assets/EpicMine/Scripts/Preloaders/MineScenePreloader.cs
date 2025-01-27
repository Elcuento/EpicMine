using System;
using System.Collections.Generic;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable IdentifierTypo

public static class MineScenePreloader
{
    private static readonly Dictionary<string, object> _preloadAssets = new Dictionary<string, object>();

    private static void PreloadAsset(string key, string path)
    {
        if (_preloadAssets.ContainsKey(key))
        {
            if (_preloadAssets[key] == null)
                _preloadAssets.Remove(key);
            else return;
        }

        var asset = Resources.Load<Object>(path);

        _preloadAssets.Add(key, asset);
    }

    public static void Preload(Action onComplete)
    {
        var settings = PlayerPrefsHelper.LoadDefault(PlayerPrefsType.PreloadQuality, 1);

        if (settings == 0)
        {
            onComplete?.Invoke();
            return;
        }

        var currentMine =
            App.Instance.Services.RuntimeStorage.Load<BlackTemple.EpicMine.Core.Mine>(RuntimeStorageKeys
                .SelectedMine);

        var currentTier =
            App.Instance.Services.RuntimeStorage.Load<BlackTemple.EpicMine.Core.Mine>(RuntimeStorageKeys
                .SelectedMine);

        /*
       if (settings >= 1)
       {
           if (!WindowManager.Instance.IsOpen<WindowCombo>())
               WindowManager.Instance.Show<WindowCombo>();

           if (!WindowManager.Instance.IsOpen<WindowVignette>())
               WindowManager.Instance.Show<WindowVignette>();

           PreloadAsset(currentTier.Tier.StaticTier.WallItem1Id, $"{Paths.ResourcesPrefabsWallsPath}{currentTier.Tier.StaticTier.WallItem1Id}");
           PreloadAsset(currentTier.Tier.StaticTier.WallItem2Id, $"{Paths.ResourcesPrefabsWallsPath}{currentTier.Tier.StaticTier.WallItem2Id}");
           PreloadAsset(currentTier.Tier.StaticTier.WallItem3Id, $"{Paths.ResourcesPrefabsWallsPath}{currentTier.Tier.StaticTier.WallItem3Id}");
       }

       if (settings >= 2)
       {
           if (App.Instance.StaticData.MineMonstersSpawn.Count > currentTier.Number)
           {
               var monsters = new List<string>();

               var tierSpawn = App.Instance.StaticData.MineMonstersSpawn[currentTier.Number];
               foreach (var spawn in tierSpawn.SpawnChances)
               {
                   foreach (var chance in spawn.Chance)
                   {
                       if (chance.Value > 0)
                       {
                           if (monsters.Contains(chance.Key))
                               continue;

                           var monster = App.Instance.StaticData.Monsters.Find(x => x.Id == chance.Key);

                           PreloadAsset(monster.Type.ToString(), $"{Paths.ResourcesPrefabsMonstersPath}{monster.Type}/{monster.Type}");

                           PreloadAsset(monster.Id, $"{Paths.ResourcesPrefabsMonstersPath}{monster.Type}/Textures/{monster.Id}");

                           monsters.Add(monster.Id);
                       }
                   }

               }
           }
       }*/

        if (settings >= 1)
        {
            var tier = currentTier.Number + 1;
            var mine = currentMine.Number + 1;

            var fieldId = $"{tier}";
            FieldHelper.LoadSpecificFieldData(fieldId);

            fieldId = $"{tier}_{mine}";
            FieldHelper.LoadSpecificFieldData(fieldId);

            for (var j = 0; j < MineLocalConfigs.DefaultSectionsCount; j++)
            {
                fieldId = $"{tier}_{mine}_{j + 1}";
                FieldHelper.LoadSpecificFieldData(fieldId);
            }
        }

        onComplete?.Invoke();
    }
}
