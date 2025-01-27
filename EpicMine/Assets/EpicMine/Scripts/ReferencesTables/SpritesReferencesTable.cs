
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    [CreateAssetMenu(fileName = "SpritesReferencesTable")]
    public class SpritesReferencesTable : ScriptableObject
    {
        public Sprite MineCompleteIcon;
        public Sprite MineIncompleteIcon;
        public Sprite MineLastIcon;

        [Header("Currencies")]
        public Sprite GoldIcon;
        public Sprite CrystalsIcon;

        [Header("Rating")]
        public Sprite StarEmptyIcon;
        public Sprite StarFullIcon;

        public Sprite SkullEmptyIcon;
        public Sprite SkullFullIcon;

        [Header("Artefact")]
        public Sprite ArtefactIcon;

        [Header("Buttons")]
        public Sprite ButtonGreen;
        public Sprite ButtonGrey;
        public Sprite ButtonGrown;
        public Sprite ButtonYellow;
        public Sprite ButtonRed;


        [Header("MainButtons")]
        public Sprite WorkShopActiveButton;
        public Sprite WorkShopInactiveButton;

        [Header("Pickaxe rarity backgrounds")]
        public Sprite Simple;
        public Sprite Rare;
        public Sprite Legendary;

        [Header("Characteristics")]
        public Sprite DamageIcon;
        public Sprite FortuneIcon;
        public Sprite CritIcon;

        [Header("Shop packs backgrounds")]
        public Sprite ShopPackCrystalsBackground;
        public Sprite[] ShopPackChestBackgrounds;
        public Sprite[] ShopPackPickaxeBackgrounds;

        [Header("Buffs")] 
        public Sprite ExplosiveStrikeAbilityIcon;
        public Sprite FreezingAbilityIcon;
        public Sprite FreezingAbilityBuffIcon;
        public Sprite FreezingAbilityAdditionalIcon;
        public Sprite AcidAbilityIcon;
        public Sprite AcidAbilityBuffIcon;
        public Sprite HealingBuffIcon;
        public Sprite AbilityCooldownIcon;
        public Sprite AbilityDamageIcon;

        [Header("Effects")]
        public Sprite[] EffectIcons;

        [Header("Prestige")]
        public Sprite[] PrestigeIcons;

        public Sprite HeartIcon;
        public Sprite EnergyIcon;

        [Header("Common")]
        public Sprite ChatIcon;
        public Sprite CloseIcon;

        [Header("Walls")]
        public Texture2D[] InnerCrack;
        public Texture2D[] OuterCrack;
        public Texture2D[] CriticalCrack;

        [Header("Quests")]
        public Sprite QuestGetIcon;
        public Sprite QuestCompleteIcon;

        [Header("Features")]
        public Feature[] Features;

        [System.Serializable]
        public struct Feature
        {
            public FeaturesType Id;
            public Sprite Resource;
        }
    }
}