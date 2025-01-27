namespace BlackTemple.EpicMine
{
    public class EndlessSectionProvider : SectionProviderBase
    {
        private readonly bool _allowChests;

        public EndlessSectionProvider(MineSceneHero hero, bool allowChests = true, ExploderPoolController pool = null) : base(hero, pool)
        {
            _allowChests = allowChests;

            var firstSectionNumber = GetNextSectionNumber();
            var firstSection = _sectionFactory.CreateWallSection();
            AddNewSection(firstSection, firstSectionNumber);

            CreateNextSection();
            Subscribe();
        }


        protected override void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            base.OnSectionPassed(eventData);
            CreateNextSection();
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

            if (CreateMonsterOrWallSection(nextSectionNumber))
                return;

            if (CreateChestOrWallSection(nextSectionNumber))
                return;

            var section = _sectionFactory.CreateWallSection();
            AddNewSection(section, nextSectionNumber);
        }
    }
}