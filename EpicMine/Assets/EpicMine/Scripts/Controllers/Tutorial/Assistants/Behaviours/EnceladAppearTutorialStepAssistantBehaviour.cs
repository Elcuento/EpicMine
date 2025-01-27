using System.Linq;
using BlackTemple.Common;
using DG.Tweening;
using DragonBones;
using UnityEngine;
// ReSharper disable IdentifierTypo

namespace BlackTemple.EpicMine
{
    public class EnceladAppearTutorialStepAssistantBehaviour : TutorialStepAssistantBehaviour
    {
        public RectTransform FlyingRect;
        public UnityArmatureComponent EnceladFlying;

        public GameObject EnceladSitting;
        public GameObject TorchesMerchant;
        public GameObject EnceladFreePlace;


        private void Awake()
        {
            Subscribe();
        }

        public void OnDestroy()
        {
            UnSubscribe();
        }

        public bool IsMineReached()
        {
            var firstTier = App.Instance.Player.Dungeon.Tiers.FirstOrDefault();

            if (firstTier?.Mines.Find(x => x.IsComplete) == null)
                return false;

            if (firstTier.Mines.FindLast(x => x.IsComplete).Number < 2)
                return false;

            return true;
        }

        private void Start()
        {

            if (App.Instance.Controllers.TutorialController.IsStepComplete(StepId))
            {
                EnceladSitting.SetActive(true);
                EnceladFreePlace.SetActive(false);
                TorchesMerchant.SetActive(true);

                EnceladFlying.gameObject.SetActive(false);
                return;
            }
            else
            {
                EnceladSitting.SetActive(false);
                EnceladFreePlace.SetActive(true);
                TorchesMerchant.SetActive(false);
            }
          

            if (IsMineReached())
            {
                EnceladFlying.gameObject.SetActive(true);
                StartFly();
            }
            else
            {
                EnceladFlying.gameObject.SetActive(false);
            }
        }

        public void StartFly()
        {
            var size = Random.Range(1, 1.4f);
            var isRight = Random.Range(0, 100) > 50;

            var startPosition = isRight ? -500 : FlyingRect.rect.width + 500;
            var endPosition = isRight ? FlyingRect.rect.width + 500 : -500;
            var yPosition = FlyingRect.transform.position.y + Random.Range(0, FlyingRect.rect.height);

            EnceladFlying.transform.localScale = new Vector3(100 * size, 100 * size, 100 * size);
            EnceladFlying.transform.localEulerAngles = new Vector3(0, isRight ? -180 : 0, 0);
            EnceladFlying.transform.localPosition = new Vector3(startPosition, yPosition);

            EnceladFlying.transform.DOLocalMoveX(endPosition, Random.Range(15,25)).SetEase(Ease.Linear)
                .OnComplete(StartFly).SetDelay(Random.Range(0, 35));
        }

        private void OnEnceladAppear(TutorialStepReadyEvent eventData)
        {
            if (eventData.Step.Id != StepId) return;
            
                EnceladFlying.gameObject.SetActive(false);
                EnceladFlying.DOKill();
            
        }

        private void OnEnceladSpeakEnd(TutorialStepCompleteEvent eventData)
        {
            if (eventData.Step.Id != StepId) return;

            EnceladFlying.gameObject.SetActive(false);
            EnceladFlying.DOKill();

            EnceladSitting.SetActive(true);
            EnceladFreePlace.SetActive(false);

            TorchesMerchant.SetActive(true);
        }

        public void Subscribe()
        {
            EventManager.Instance.Subscribe<TutorialStepReadyEvent>(OnEnceladAppear);
            EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnEnceladSpeakEnd);
        }

        public void UnSubscribe()
        {
            if (EventManager.Instance == null) return;

            EventManager.Instance.Unsubscribe<TutorialStepReadyEvent>(OnEnceladAppear);
            EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnEnceladSpeakEnd);
        }
    }
}