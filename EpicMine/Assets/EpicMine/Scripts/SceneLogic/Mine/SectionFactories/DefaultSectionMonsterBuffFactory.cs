
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class DefaultMonsterBuffFactory : ISectionMonsterBuffFactory
    {
        private readonly MineSceneMonsterSection _monsterSection;


        public DefaultMonsterBuffFactory(MineSceneMonsterSection monster)
        {
            _monsterSection = monster;
        }

        public MineSceneMonsterSelfBuff CreateSelfBuff(MonsterAbility ability)
        {
            MineSceneMonsterSelfBuff buff;

            var prefab = Resources.Load<MineSceneMonsterSelfBuff>($"{Paths.ResourcesPrefabsHeroBuffsPath}{ability.Id}");

            if (prefab == null)
            {
                buff = new GameObject(ability.Id)
                    .AddComponent<MineSceneMonsterSelfBuff>();
            }
            else
            {
                buff = Object.Instantiate(prefab, _monsterSection.transform, false);
            }

            buff.Initialize(ability, _monsterSection);
            return buff;
        }

        public MineSceneMonsterOtherBuff CreateOtherBuff(MonsterAbility ability, MineSceneHero hero)
        {
            MineSceneMonsterOtherBuff buff;

            var prefab = Resources.Load<MineSceneMonsterOtherBuff>($"{Paths.ResourcesPrefabsHeroBuffsPath}{ability.Id}");

            if (prefab == null)
            {
                buff = new GameObject(ability.Id)
                    .AddComponent<MineSceneMonsterOtherBuff>();
            }
            else
            {
                buff = Object.Instantiate(prefab, hero.transform, false);
            }
      
            buff.Initialize(ability, _monsterSection, hero);
            return buff;
        }
    }
}