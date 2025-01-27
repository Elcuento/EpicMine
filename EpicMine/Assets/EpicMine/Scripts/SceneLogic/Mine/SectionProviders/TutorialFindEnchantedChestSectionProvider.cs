

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public class TutorialFindEnchantedChestSectionProvider : SectionProviderBase
    {
        private readonly bool _allowChests;
        private readonly int _sectionsCount;

        public TutorialFindEnchantedChestSectionProvider(MineSceneHero hero, bool allowChests = true, ExploderPoolController pool = null) : base(hero, pool)
        {
            _allowChests = allowChests;
            _sectionsCount = MineLocalConfigs.DefaultSectionsCount;

            var firstSectionNumber = GetNextSectionNumber();
            var firstSection = _sectionFactory.CreateWallSection();
            AddNewSection(firstSection, firstSectionNumber);
            
            CreateNextSection();
            Subscribe();
        }


        protected override void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            base.OnSectionPassed(eventData);

            var nextSectionNumber = GetNextSectionNumber();

            if (nextSectionNumber < _sectionsCount)
            {
                CreateNextSection();
                return;
            }

            var doorSection = _sectionFactory.CreateDoorSection();
            AddNewSection(doorSection, nextSectionNumber);

            CreateEmptySection();
            CreateEmptySection();

            Unsubscribe();

        }

        private bool CreateMonsterOrWallSection(int sectionNumber)
        {
            if (sectionNumber <= 0)
                return false;

            var isLastMonster = Sections[Sections.Count - 1] as MineSceneMonsterSection;

            if (isLastMonster != null)
                return false;

            var section = _sectionFactory.CreateMonsterOrNullSection();

            if (section != null)
            {
                CreateEmptySection();
                sectionNumber = GetNextSectionNumber();
                AddNewSection(section, sectionNumber);

                return true;
            }

            return false;
        }

        private bool CreateChestOrWallSection(int nextSectionNumber)
        {
            if (!_allowChests)
                return false;

            var section = _sectionFactory.CreateWallOrChestSection();

            if (section is MineSceneChestSection)
            {
                CreateEmptySection();
                CreateEmptySection();
                nextSectionNumber = GetNextSectionNumber();
            }

            AddNewSection(section, nextSectionNumber);
            return true;
        }


        private void CreateNextSection()
        {
            var nextSectionNumber = GetNextSectionNumber();

            if (nextSectionNumber >= MineLocalConfigs.TutorialFindEnchantedChestSectionPositionNumber)
            {
                var savedState = PlayerPrefsHelper.LoadDefault(PlayerPrefsType.TutorialEnchancedChestFourLevel, false);

                if (!savedState)
                {
                    var newSectionNumber = GetNextSectionNumber();
                    var newSection = _sectionFactory.CreateChestSection();
                    AddNewSection(newSection, newSectionNumber);

                    PlayerPrefsHelper.Save(PlayerPrefsType.TutorialEnchancedChestFourLevel, true);
                    return;
                }
            }

            if (CreateMonsterOrWallSection(nextSectionNumber))
                return;

            if (CreateChestOrWallSection(nextSectionNumber))
                return;

            var section = _sectionFactory.CreateWallSection();
            AddNewSection(section, nextSectionNumber);
        }
    }
}