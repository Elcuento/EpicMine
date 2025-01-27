using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class FirstEnergyAbilityTutorialStep : TutorialStepBase
    {
        public FirstEnergyAbilityTutorialStep(bool isComplete) : base(TutorialStepIds.FirstEnergyAbility, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }


        public override void CheckReady()
        {

            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Mine)
                return;

            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            if (selectedTier.Number != 0)
                return;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (selectedMine.Number != 3)
                return;

             SetReady();
        }

        public override void Clear()
        {
            base.Clear();
            Unsubscribe();
        }


        protected override void OnReady()
        {
            SetComplete();
        }

        protected override void OnComplete()
        {
            Unsubscribe();

            WindowManager
                .Instance
                .Show<WindowNewEnergyAbility>(withPause: true)
                .Initialize(AbilityType.ExplosiveStrike, () =>
                    {
                        WindowManager.Instance.Show<WindowFirstAbilityTutorialStepAssistant>(withPause:true);
                    });
        }


        private void EnergyChange(MineSceneEnergyChangeEvent eventData)
        {
            if(eventData.Energy >= App.Instance.Player.Abilities.ExplosiveStrike.StaticLevel.EnergyCost)
               CheckReady();
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<MineSceneEnergyChangeEvent>(EnergyChange);

        }

        private void Unsubscribe()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<MineSceneEnergyChangeEvent>(EnergyChange);
        }
    }
}