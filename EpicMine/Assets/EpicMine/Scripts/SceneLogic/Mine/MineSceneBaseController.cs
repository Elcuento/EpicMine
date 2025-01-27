using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneBaseController : MonoBehaviour
    {
        public SectionProviderBase SectionProvider { get; protected set; }

        public MineSceneHero Hero => _hero;

        [SerializeField] protected MineSceneHero _hero;

        protected bool _isGameEnd;

        protected virtual void Awake()
        {
            Subscribe();
        }


        protected virtual void Start()
        {  }

        protected virtual void OnDestroy()
        {
            SectionProvider?.Clear();
            SectionProvider = null;
            Unsubscribe();
        }

        protected virtual void Initialize()
        {
            CreateSectionProvider();

            SectionProvider.Sections.FirstOrDefault()?.SetReady();
        }

        protected virtual void Subscribe()
        {
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Subscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeDestroyed);
        }

        protected virtual void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
                EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                EventManager.Instance.Unsubscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeDestroyed);
            }
        }

        protected virtual void CreateSectionProvider()
        {  }


        protected virtual void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {  }

        protected virtual IEnumerator MoveHero(float delay, MineSceneSection passedSection)
        {
            yield return new WaitForSeconds(delay);
        }

        protected virtual void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            var passedSections = SectionProvider.Sections.Where(s =>
                    (s.IsPassed || s is MineSceneEmptySection) && s.Number < (_hero.CurrentSection != null ? _hero.CurrentSection.Number : 0))
                .ToList();
            foreach (var passedSection in passedSections)
            {
                SectionProvider.Sections.Remove(passedSection);
                Destroy(passedSection.gameObject);
            }
        }

        protected virtual void OnPickaxeDestroyed(MineScenePickaxeDestroyedEvent data)
        {}

    }
}