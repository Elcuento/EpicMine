using System.Collections.Generic;
using CommonDLL.Dto;
using CommonDLL.Static;


namespace BlackTemple.EpicMine.Dto
{
    public struct PlayerDataUpdatePack
    {
        public List<Item> Items;

        public List<CommonDLL.Dto.Currency> Currencies;

        public Dictionary<SkillType, int> Skills;

        public Dictionary<AbilityType, int> Abilities;

        public Dictionary<AutoMinerUpgradeType, int> AutoMinerUpgrades;

        public List<CommonDLL.Dto.Pickaxe> Pickaxes;

        public List<CommonDLL.Dto.Torch> Torches;

        public Dictionary<string, int> AdPickaxes;

        public Dictionary<string, int> AdTorches;

        public List<CommonDLL.Dto.Tier> Tiers;

        public List<CommonDLL.Dto.Quest> Quests;

        public List<CommonDLL.Dto.Recipe> Recipes;

        public string SelectedPickaxe;

        public string SelectedTorch;

        public TutorialStepIds? TutorialStepId;

        public int PvpInviteDisable;

        public PlayerDataUpdatePack(List<Item> items, List<CommonDLL.Dto.Currency> currencies, Dictionary<SkillType, int> skills,
            Dictionary<AbilityType, int> abilities, List<CommonDLL.Dto.Pickaxe> pickaxes,
            List<CommonDLL.Dto.Torch> torches,
            Dictionary<string, int> adPickaxes,
            Dictionary<string, int> adTorches,
            List<CommonDLL.Dto.Tier> tiers, List<CommonDLL.Dto.Recipe> recipes,
            string selectedPickaxe, string selectedTorch, TutorialStepIds? tutorialStepId, int pvpInviteDisable, Dictionary<AutoMinerUpgradeType, int> autoMinerUpgrades, List<CommonDLL.Dto.Quest> quests)
        {
            Items = items;
            Currencies = currencies;
            Skills = skills;
            Abilities = abilities;
            Pickaxes = pickaxes;
            Torches = torches;
            AdPickaxes = adPickaxes;
            AdTorches = adTorches;
            Tiers = tiers;
            Recipes = recipes;
            SelectedPickaxe = selectedPickaxe;
            SelectedTorch = selectedTorch;
            TutorialStepId = tutorialStepId;
            PvpInviteDisable = pvpInviteDisable;
            AutoMinerUpgrades = autoMinerUpgrades;
            Quests = quests;
        }
    }
}