using System;
using System.Collections;
using BlackTemple.Common;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class MineSceneHeroMoveController :  MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private const string MoveAnimationName = "Move";

        private const string IdleAnimationName = "Idle";


        public void Move(float newPosition, float duration, Action onComplete = null, Action onStart = null)
        {
            EventManager.Instance.Publish(new MineSceneHeroMoveEvent(true));
            transform
                .DOMoveZ(newPosition, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => { onComplete?.Invoke(); })
                .OnStart(()=> onStart?.Invoke());

            StopAllCoroutines();
            StartCoroutine(Move(duration));
        }


        private IEnumerator Move(float duration)
        {
            var soundsCoroutine = StartCoroutine(PlayStepsSounds());
            _animator.CrossFade(MoveAnimationName, 0.3f);

            yield return new WaitForSeconds(duration);

            StopCoroutine(soundsCoroutine);
            _animator.CrossFade(IdleAnimationName, 0.3f);
        }

        private IEnumerator PlayStepsSounds()
        {
            while (true)
            {
                var randomStepIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.Steps.Length);
                var randomStep = App.Instance.ReferencesTables.Sounds.Steps[randomStepIndex];
                AudioManager.Instance.PlaySound(randomStep);
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}