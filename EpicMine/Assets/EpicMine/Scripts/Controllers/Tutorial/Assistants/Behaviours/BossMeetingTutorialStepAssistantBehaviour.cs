using System.Linq;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class BossMeetingTutorialStepAssistantBehaviour : TutorialStepAssistantBehaviour
    {
        public GameObject ArrowToThirdMine;
        public GameObject ArrowToFourthMine;
        public GameObject ArrowToFiveMine;

        private void Start()
        {
            if (App.Instance.Controllers.TutorialController.IsStepComplete(StepId))
                return;

            if (WithCompletedSteps != null)
            {
                foreach (var completedStep in WithCompletedSteps)
                {
                    if (!App.Instance.Controllers.TutorialController.IsStepComplete(completedStep))
                        return;
                }
            }

            var tier = App.Instance.Player.Dungeon.Tiers.FirstOrDefault();
            var lastCompleteMineNumber = tier.Mines.LastOrDefault(m => m.IsComplete).Number;

            switch (lastCompleteMineNumber)
            {
                case 1:
                    ArrowToThirdMine.SetActive(true);
                    break;
                case 2:
                    ArrowToFourthMine.SetActive(true);
                    break;
                case 3:
                    ArrowToFiveMine.SetActive(true);
                    break;
            }
        }
    }
}