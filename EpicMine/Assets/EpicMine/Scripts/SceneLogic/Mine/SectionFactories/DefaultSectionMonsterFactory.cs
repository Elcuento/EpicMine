
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class DefaultSectionMonsterFactory : ISectionMonsterFactory
    {
        private readonly Transform _container;

        public DefaultSectionMonsterFactory(Transform container)
        {
            _container = container;
        }

        public MineSceneMonster CreateMonster(MonsterType type, string id)
        {
            return CreateSectionMonster(type, id);
        }

        private MineSceneMonster CreateSectionMonster(MonsterType type, string id)
        {
            var prefab = Resources.Load<MineSceneMonster>($"{Paths.ResourcesPrefabsMonstersPath}{type}/{type}");
            var texture = Resources.Load<Texture>($"{Paths.ResourcesPrefabsMonstersPath}{type}/Textures/{id}");

            var monster = Object.Instantiate(prefab, _container, false);

            if(texture != null)
            monster.InitializeTexture(texture);
            else App.Instance.Services.LogService.LogWarning($"{id} Texture not exist");

            return monster;
        }

    }
}