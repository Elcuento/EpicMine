
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class LimitedSectionProvider : SectionProviderBase
    {
        private readonly int _sectionsCount;

        private readonly bool _allowChests;


        public LimitedSectionProvider(int sectionsCount, MineSceneHero hero, bool allowChests = true) : base(hero)
        {
            _sectionsCount = sectionsCount;
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


        private bool CreateMonsterOrNullSection(int sectionNumber)
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
          //  CreateSection(SectionType.Monster, "elemental_4");

        //    return;

              var nextSectionNumber = GetNextSectionNumber();

            if (CreateMonsterOrNullSection(nextSectionNumber))
                return;

            if (CreateChestOrWallSection(nextSectionNumber))
                return;

            var section = _sectionFactory.CreateWallSection();

            AddNewSection(section, nextSectionNumber);
        }
    }
}