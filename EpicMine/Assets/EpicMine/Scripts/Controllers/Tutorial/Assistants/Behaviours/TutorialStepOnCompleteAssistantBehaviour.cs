using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class TutorialStepOnCompleteAssistantBehaviour : TutorialStepAssistantBehaviour
    {
        [Space]
        public GameObject[] ObjectsToDisable;
        public GameObject[] ObjectsToEnable;

        [Space]
        public Behaviour[] ComponentsToDisable;
        public Behaviour[] ComponentsToEnable;

        [Space]
        public CanvasGroupFadeSettings[] CanvasGroupsToFade;


        private void Awake()
        {
            if (App.Instance.Controllers.TutorialController.IsStepComplete(StepId))
                return;

            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }


        private void OnStepComplete(TutorialStepCompleteEvent eventData)
        {
            if (eventData.Step.Id == StepId)
            {
                Unsubscribe();

                if (ObjectsToDisable != null)
                {
                    foreach (var o in ObjectsToDisable)
                        o.SetActive(false);
                }

                if (ObjectsToEnable != null)
                {
                    foreach (var o in ObjectsToEnable)
                        o.SetActive(true);
                }

                if (ComponentsToDisable != null)
                {
                    foreach (var c in ComponentsToDisable)
                        c.enabled = false;
                }

                if (ComponentsToEnable != null)
                {
                    foreach (var c in ComponentsToEnable)
                        c.enabled = true;
                }

                if (CanvasGroupsToFade != null)
                {
                    foreach (var canvasGroup in CanvasGroupsToFade)
                        canvasGroup.CanvasGroup.alpha = canvasGroup.Fade;
                }
            }
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnStepComplete);
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnStepComplete);
        }
    }
}