using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class VillageSceneCharacter : MonoBehaviour
    {
        [SerializeField] protected VillageSceneController _controller;

        [SerializeField] protected CharacterType _type;

        [Space]
        [SerializeField] protected string _idleAnimationName;
        [SerializeField] protected List<string> _randomAnimationNames = new List<string>();
        [SerializeField] protected List<string> _loopAnimationNames = new List<string>();
        [SerializeField] protected UnityArmatureComponent _armature;

        [Space]
        [SerializeField] protected GameObject _questArrow;
        [SerializeField] protected Image _questArrowIcon;

        private Core.QuestTask _attachedTask;
        private Core.Quest _attachedQuest;

        protected virtual void Awake()
        {
            Subscribe();
        }

        protected virtual void Start()
        {
            if (_randomAnimationNames.Count == 0 || _loopAnimationNames.Count == 0)
            {
                if(_armature != null)
                _armature.animation.FadeIn(_idleAnimationName, 0.1f);
            }
            else
            {
                StartCoroutine(AnimationCycle());
            }
              
            _controller.RegisterVillageQuestCharacter(this, _questArrow);

            CheckQuest();
        }

        protected virtual void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            UnSubscribe();
        }


        protected virtual void Subscribe()
        {
            EventManager.Instance.Subscribe<QuestTaskCompleteEvent>(OnTaskCompleted);
            EventManager.Instance.Subscribe<QuestTaskGoalCompleteEvent>(OnGoalCompleted);
            EventManager.Instance.Subscribe<QuestCompleteEvent>(OnQuestComplete);
            EventManager.Instance.Subscribe<QuestStartEvent>(OnQuestStart);
            EventManager.Instance.Subscribe<QuestActivateEvent>(OnQuestActivate);

        }


        protected virtual void UnSubscribe()
        {
            EventManager.Instance.Unsubscribe<QuestTaskCompleteEvent>(OnTaskCompleted);
            EventManager.Instance.Unsubscribe<QuestTaskGoalCompleteEvent>(OnGoalCompleted);
            EventManager.Instance.Unsubscribe<QuestCompleteEvent>(OnQuestComplete);
            EventManager.Instance.Unsubscribe<QuestStartEvent>(OnQuestStart);
            EventManager.Instance.Unsubscribe<QuestActivateEvent>(OnQuestActivate);

        }

        public void OnClick()
        {
            if (_attachedTask != null)
            {
                _attachedTask.Speak(_type);
            }
            else if(_attachedQuest != null)
            {
                _attachedQuest.Speak(_type);
            }
            else
            {
                _controller.ClickCharacter(_type);
            }
        }

        public void OnClickQuestTyp()
        {
            _controller.ScrollToCharacter(_type, () =>
            {
                if (_attachedTask != null)
                {
                    _attachedTask.Speak(_type);
                }
                else if (_attachedQuest != null)
                {
                    _attachedQuest.Speak(_type);
                }
            });


        }

        protected virtual void OnTaskCompleted(QuestTaskCompleteEvent data)
        {
            CheckQuest();
        }

        protected virtual void OnGoalCompleted(QuestTaskGoalCompleteEvent eventData)
        {
            CheckQuest();
        }

        protected virtual void OnQuestComplete(QuestCompleteEvent eventData)
        {
            CheckQuest();
        }

        protected virtual void OnQuestStart(QuestStartEvent eventData)
        {
            CheckQuest();
        }

        protected virtual void OnQuestActivate(QuestActivateEvent eventData)
        {
            CheckQuest();
        }

        protected virtual void CheckQuest()
        {
            var isActive = false;

            _attachedQuest = App.Instance.Player.Quests.IsExistStartQuest(_type);

            if (_attachedQuest != null)
            {
                _questArrowIcon.sprite = App.Instance.ReferencesTables.Sprites.QuestGetIcon;
                isActive = true;
            }
            else
            {
                _attachedTask = App.Instance.Player.Quests.IsExistInSpeakQuest(_type);

                if (_attachedTask != null && !_attachedTask.IsReady)
                {
                    _questArrowIcon.sprite = App.Instance.ReferencesTables.Sprites.QuestCompleteIcon;
                    isActive = true;
                }
            }

            _questArrow.transform.DOKill();

            if (isActive)
            {
                _questArrow.transform.DOLocalMoveY(_questArrow.transform.localPosition.y + 25f, 2)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            _questArrow.SetActive(isActive);

        }

        protected virtual IEnumerator LoopAnimationCycle()
        {
            if (_loopAnimationNames.Count < 3)
            {
                StartCoroutine(AnimationCycle());
                yield break;
            }

            var animIndex = 0;

            _armature.animation.FadeIn(_idleAnimationName, 0.1f);

            var randomTime = Random.Range(5f, 10f);

            yield return new WaitForSecondsRealtime(randomTime);

            var loopAnimationName = _loopAnimationNames[animIndex];
            _armature.animation.FadeIn(loopAnimationName, 0.1f, 1);

            yield return new WaitForSecondsRealtime(_armature.animation.animations[loopAnimationName].duration);
            animIndex++;

            loopAnimationName = _loopAnimationNames[animIndex];
            _armature.animation.FadeIn(loopAnimationName, 0.1f, 1);

            randomTime = Random.Range(10f, 20f);
            yield return new WaitForSecondsRealtime(randomTime);
            animIndex++;

            loopAnimationName = _loopAnimationNames[animIndex];
            _armature.animation.FadeIn(loopAnimationName, 0.1f, 1);

            yield return new WaitForSecondsRealtime(_armature.animation.animations[loopAnimationName].duration);

            StartCoroutine(AnimationCycle());

        }

        protected virtual IEnumerator AnimationCycle()
        {
            if (Random.Range(0, 100) > 50)
            {
                StartCoroutine(LoopAnimationCycle());
                yield break;
            }

            _armature.animation.FadeIn(_idleAnimationName, 0.1f);

            var randomTime = Random.Range(5f, 10f);
            yield return new WaitForSecondsRealtime(randomTime);

            var randomAnimationIndex = Random.Range(0, _randomAnimationNames.Count);
            var randomAnimationName = _randomAnimationNames[randomAnimationIndex];

            _armature.animation.FadeIn(randomAnimationName, 0.1f, 1);
            yield return new WaitForSecondsRealtime(_armature.animation.animations[randomAnimationName].duration);

            StartCoroutine(AnimationCycle());
        }
    }
}