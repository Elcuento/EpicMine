
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class BossSectionProvider : SectionProviderBase
    {
        public BossSectionProvider(MineSceneHero hero) : base(hero)
        {
            var firstSectionNumber = GetNextSectionNumber();
            var firstSection = _sectionFactory.CreateEmptySection();
            AddNewSection(firstSection, firstSectionNumber);

            var bossSectionNumber = GetNextSectionNumber();
            var bossSection = _sectionFactory.CreateBossSection();
            AddNewSection(bossSection, bossSectionNumber);

            Subscribe();
        }
        
        protected override void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            base.OnSectionPassed(eventData);

            if (eventData.Section is MineSceneBossSection)
            {
                for (var i = eventData.Section.Number + 1; i < MineLocalConfigs.BossSectionsCount; i++)
                {
                    var savedState = PlayerPrefsHelper.LoadDefault(PlayerPrefsType.TutorialEnchancedChestAfterBoss, false);

                    if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier) && !savedState)
                    {
                        var newSectionNumber = GetNextSectionNumber();
                        var newSection = _sectionFactory.CreateChestSection();
                        AddNewSection(newSection, newSectionNumber);

                        var newSectionChest = newSection as MineSceneChestSection;

                        if (newSectionChest != null)
                        {
                            newSectionChest.SetChest(ChestType.Royal);
                        }

                        PlayerPrefsHelper.Save(PlayerPrefsType.TutorialEnchancedChestAfterBoss, true);
                    }
                    else
                    {
                        var newSectionNumber = GetNextSectionNumber();
                        var newSection = _sectionFactory.CreateEmptyOrChestSection();
                        AddNewSection(newSection, newSectionNumber);
                    }
                }

                var doorSectionNumber = GetNextSectionNumber();
                var doorSection = _sectionFactory.CreateLastDoorSection();
                AddNewSection(doorSection, doorSectionNumber);

                CreateEmptySection();
                CreateEmptySection();

                Unsubscribe();
            }
        }
    }
}