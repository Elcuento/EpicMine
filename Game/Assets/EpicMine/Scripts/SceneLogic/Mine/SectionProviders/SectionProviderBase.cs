using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BlackTemple.EpicMine
{
    public abstract class SectionProviderBase
    {
        public List<MineSceneSection> Sections { get; } = new List<MineSceneSection>();

        protected readonly MineSceneHero _hero;

        protected readonly ISectionFactory _sectionFactory;


        protected SectionProviderBase(MineSceneHero hero, bool isPvp = false)
        {
            _hero = hero;
            var container = new GameObject("Sections").transform;

            if (isPvp) _sectionFactory =  new PvpSectionFactory(container);
            else _sectionFactory = new DefaultSectionFactory(container);
        }

        public virtual void Clear()
        {
            Unsubscribe();
        }

        public virtual void CreateSection(SectionType type, string id)
        {
            var nextSectionNumber = GetNextSectionNumber();

            switch (type)
            {
                case SectionType.Etc:

                    var section = _sectionFactory.CreateEmptySection();
                    AddNewSection(section, nextSectionNumber);
                    break;
                case SectionType.Wall:
                    var section2 = _sectionFactory.CreateWallSection();
                    AddNewSection(section2, nextSectionNumber);
                    break;
                case SectionType.Monster:
                    var section3 = _sectionFactory.CreateMonsterSection(id);
                    AddNewSection(section3, nextSectionNumber);
                    break;
                case SectionType.Boss:
                    var section4 = _sectionFactory.CreateBossSection();
                    AddNewSection(section4, nextSectionNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public virtual void ExchangeLastSection(MineSceneSection sectionSource, SectionType type, string id)
        {
            Object.Destroy(sectionSource.gameObject);
            Sections.Remove(sectionSource);
            var nextSectionNumber = GetNextSectionNumber();

            switch (type)
            {
                case SectionType.Etc:

                    var section = _sectionFactory.CreateEmptySection();
                    AddNewSection(section, nextSectionNumber);
                    break;
                case SectionType.Wall:
                    var section2 = _sectionFactory.CreateWallSection();
                    AddNewSection(section2, nextSectionNumber);
                    break;
                case SectionType.Monster:
                    var section3 = _sectionFactory.CreateMonsterSection(id);
                    AddNewSection(section3, nextSectionNumber);
                    break;
                case SectionType.Boss:
                    var section4 = _sectionFactory.CreateBossSection();
                    AddNewSection(section4, nextSectionNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        protected virtual void Subscribe()
        {
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
        }

        protected virtual void Unsubscribe()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
        }

        protected virtual void OnSectionPassed(MineSceneSectionPassedEvent eventData) { }


        protected int GetNextSectionNumber()
        {
            return Sections.Count > 0
                ? Sections.Last().Number + 1
                : 0;
        }

        protected void AddNewSection(MineSceneSection section, int number)
        {
            section.transform.localPosition = Vector3.forward * number * MineLocalConfigs.SectionSize;
            section.Initialize(number, _hero);
            Sections.Add(section);
        }

        protected void CreateEmptySection()
        {
            var nextSectionNumber = GetNextSectionNumber();
            var newSection = _sectionFactory.CreateEmptySection();
            AddNewSection(newSection, nextSectionNumber);
        }
    }
}