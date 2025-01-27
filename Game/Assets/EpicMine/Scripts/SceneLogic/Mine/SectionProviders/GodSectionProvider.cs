namespace BlackTemple.EpicMine
{
    public class GodSectionProvider : SectionProviderBase
    {
        public GodSectionProvider(MineSceneHero hero) : base(hero)
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
                for (var i = eventData.Section.Number + 1; i < MineLocalConfigs.GodSectionPositionNumber; i++)
                {
                    var newSectionNumber = GetNextSectionNumber();
                    var newSection = _sectionFactory.CreateEmptySection();
                    AddNewSection(newSection, newSectionNumber);
                }

                var godSectionNumber = GetNextSectionNumber();
                var godSection = _sectionFactory.CreateGodSection();
                AddNewSection(godSection, godSectionNumber);

                CreateEmptySection();
                CreateEmptySection();

                Unsubscribe();
            }
        }
    }
}