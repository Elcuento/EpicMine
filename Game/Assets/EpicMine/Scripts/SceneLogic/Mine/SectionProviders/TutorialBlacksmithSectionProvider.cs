namespace BlackTemple.EpicMine
{
    public class TutorialBlacksmithSectionProvider : SectionProviderBase
    {
        public TutorialBlacksmithSectionProvider(MineSceneHero hero) : base(hero)
        {
            CreateNextSection();
            CreateNextSection();

            Subscribe();
        }


        protected override void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            base.OnSectionPassed(eventData);
            var nextSectionNumber = GetNextSectionNumber();
            if (nextSectionNumber < MineLocalConfigs.BlacksmithSectionPositionNumber)
            {
                CreateNextSection();
                return;
            }

            var blacksmithSection = _sectionFactory.CreateBlacksmithSection();
            AddNewSection(blacksmithSection, nextSectionNumber);

            CreateEmptySection();
            CreateEmptySection();

            Unsubscribe();
        }

        private void CreateNextSection()
        {
            var nextSectionNumber = GetNextSectionNumber();
            var newSection =  _sectionFactory.CreateWallSection();
            AddNewSection(newSection, nextSectionNumber);
        }
    }
}