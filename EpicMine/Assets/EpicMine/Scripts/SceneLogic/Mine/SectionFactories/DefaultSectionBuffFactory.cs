
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class DefaultSectionBuffFactory : ISectionBuffFactory
    {
        private readonly MineSceneSection _section;


        public DefaultSectionBuffFactory(MineSceneSection section)
        {
            _section = section;
        }

        public MineSceneSectionBuff CreateExplosiveStrikeBuff()
        {
            return CreateBuff("ExplosiveStrike", AbilityType.ExplosiveStrike);
        }

        public MineSceneSectionBuff CreateFreezingBuff()
        {
            return CreateBuff("Freezing", AbilityType.Freezing);
        }

        public MineSceneSectionBuff CreateAcidBuff()
        {
            return CreateBuff("Acid", AbilityType.Acid);
        }

        public MineSceneSectionBuff CreateTntBuff()
        {
            return CreateBuff("TntExplosive", AbilityType.Tnt);
        }

        private MineSceneSectionBuff CreateBuff(string path, AbilityType type)
        {
            var prefab = Resources.Load<MineSceneSectionBuff>($"{Paths.ResourcesPrefabsSectionsBuffsPath}{path}");
            var buff = Object.Instantiate(prefab, _section.transform, false);
            buff.Initialize(_section, type);
            return buff;
        }
    }
}