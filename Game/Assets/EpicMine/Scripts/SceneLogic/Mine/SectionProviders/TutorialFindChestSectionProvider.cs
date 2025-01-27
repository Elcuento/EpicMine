namespace BlackTemple.EpicMine
{
    public class TutorialFindChestSectionProvider : SectionProviderBase
    {
        private readonly int _sectionsCount;


        public TutorialFindChestSectionProvider(MineSceneHero hero) : base(hero)
        {
            CreateNextSection();
            CreateNextSection();

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            _sectionsCount = MineLocalConfigs.BlacksmithSectionPositionNumber + selectedMine.Number;

            Subscribe();
        }


        protected override void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            base.OnSectionPassed(eventData);

            var nextSectionNumber = GetNextSectionNumber();
            if (nextSectionNumber < _sectionsCount)
            {
                if (nextSectionNumber == MineLocalConfigs.TutorialFindChestSectionPositionNumber)
                {
                    var newSection = _sectionFactory.CreateChestSection();
                    AddNewSection(newSection, nextSectionNumber);
                }
                else
                    CreateNextSection();

                return;
            }

            var doorSection = _sectionFactory.CreateDoorSection();
            AddNewSection(doorSection, nextSectionNumber);

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