using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class DefaultHeroBuffFactory : IHeroBuffFactory
    {
        private readonly MineSceneHero _hero;


        public DefaultHeroBuffFactory(MineSceneHero hero)
        {
            _hero = hero;
        }

        public MineSceneHeroBuff CreateDamagePotionBuff()
        {
            return CreateBuff("DamagePotion");
        }

        public MineSceneHeroBuff CreateAcidBuff()
        {
            return CreateBuff("AcidBuff");
        }

        public MineSceneHeroBuff CreatePrestigeBuff()
        {
            return CreateBuff("PrestigeBuff");
        }

        private MineSceneHeroBuff CreateBuff(string path)
        {
            var prefab = Resources.Load<MineSceneHeroBuff>($"{Paths.ResourcesPrefabsHeroBuffsPath}{path}");
            var buff = Object.Instantiate(prefab, _hero.transform, false);
            buff.Initialize(_hero);
            return buff;
        }
    }
}