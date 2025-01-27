namespace BlackTemple.EpicMine
{
    public class PvpSectionProvider : SectionProviderBase
    {
        private readonly int _sectionsCount;

        public PvpSectionProvider(int sectionsCount, MineSceneHero hero, bool isPvp = true)
            : base(hero, isPvp)
        {
            _sectionsCount = sectionsCount;

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

            var chestSection = _sectionFactory.CreateChestSection();
            AddNewSection(chestSection, nextSectionNumber);

            CreateEmptySection();
            CreateEmptySection();

            Unsubscribe();
        }


        private void CreateNextSection()
        {
            var nextSectionNumber = GetNextSectionNumber();

            var newSection = _sectionFactory.CreateWallSection();

            AddNewSection(newSection, nextSectionNumber);
        }
    }
}