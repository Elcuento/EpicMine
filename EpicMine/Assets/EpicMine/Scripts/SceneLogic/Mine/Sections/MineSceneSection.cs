using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public abstract class MineSceneSection : MonoBehaviour
    {
        public SectionType SectionType { get; protected set; }

        public bool IsPassed { get; private set; }

        public bool IsReady { get; private set; }

        public bool IsAppear { get; private set; }

        public bool IsEntered { get; private set; }

        public bool IsExit { get; private set; }

        public int Number { get; private set; }

        public MineSceneHero Hero { get; protected set; }

        public List<MineSceneSectionBuff> Buffs { get; protected set; } = new List<MineSceneSectionBuff>();

        [SerializeField] protected Collider _collider;

        protected ISectionBuffFactory _buffFactory;


        public virtual void Initialize(int number, MineSceneHero hero)
        {
            Clear();
            Hero = hero;
            Number = number;

            _buffFactory = new DefaultSectionBuffFactory(this);
        }

        public virtual void SetReady()
        {
            IsReady = true;
            EventManager.Instance.Publish(new MineSceneSectionReadyEvent(this));

            if (_collider != null)
                _collider.enabled = true;

            OnSetReady();
        }

        public virtual void SetAppear()
        {
            IsAppear = true;
            EventManager.Instance.Publish(new MineSceneSectionAppearEvent(this));
        }

        public virtual void SetEnter()
        {
            IsEntered = true;
            EventManager.Instance.Publish(new MineSceneSectionEnteredEvent(this));
        }

        public virtual void SetExit()
        {     
            IsExit = true;
            EventManager.Instance.Publish(new MineSceneSectionExitEvent(this));
        }

        public virtual bool AddBuff(AbilityType abilityType) { return false; }

        public virtual void RemoveBuff(AbilityType abilityType) { }

        public virtual void DestroySection() { }

        public void FireBuffsChangeEvent()
        {
            EventManager.Instance.Publish(new MineSceneSectionBuffsChangeEvent(this));
        }


        protected virtual void SetPassed(float delay = MineLocalConfigs.OtherSectionMoveDelay)
        {
            IsReady = false;
            IsPassed = true;
            
            EventManager.Instance.Publish(new MineSceneSectionPassedEvent(this, delay));

            if (_collider != null)
                _collider.enabled = false;
        }

        protected virtual void OnSetReady()
        {

        }

        protected virtual void Clear()
        {
            IsPassed = false;
            IsReady = false;
            IsEntered = false;
            Number = 0;

            if (_collider != null)
                _collider.enabled = false;
        }
    }
}