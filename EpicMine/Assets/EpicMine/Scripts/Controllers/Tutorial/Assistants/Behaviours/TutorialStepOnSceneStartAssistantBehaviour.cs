using System;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class TutorialStepOnSceneStartAssistantBehaviour : TutorialStepAssistantBehaviour
    {
        public bool IfStepIsComplete;

        [Space]
        public GameObject[] ObjectsToDisable;
        public GameObject[] ObjectsToEnable;

        [Space]
        public Behaviour[] ComponentsToDisable;
        public Behaviour[] ComponentsToEnable;

        [Space]
        public CanvasGroupFadeSettings[] CanvasGroupsToFade;

        private void Start()
        {
            if (IfStepIsComplete != App.Instance.Controllers.TutorialController.IsStepComplete(StepId))
                return;

            if (WithCompletedSteps != null)
            {
                foreach (var completedStep in WithCompletedSteps)
                {
                    if (!App.Instance.Controllers.TutorialController.IsStepComplete(completedStep))
                        return;
                }
            }
            
            if (WithIncompletedSteps != null)
            {
                foreach (var incompleteStep in WithIncompletedSteps)
                {
                    if (App.Instance.Controllers.TutorialController.IsStepComplete(incompleteStep))
                        return;
                }
            }

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

    [Serializable]
    public struct CanvasGroupFadeSettings
    {
        public CanvasGroup CanvasGroup;
        [Range(0, 1f)]
        public float Fade;
    }
}