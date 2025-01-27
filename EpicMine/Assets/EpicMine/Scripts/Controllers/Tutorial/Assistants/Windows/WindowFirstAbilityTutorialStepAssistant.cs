using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public class WindowFirstAbilityTutorialStepAssistant : WindowBase
    {
        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            Subscribe();
        }

        public override void OnClose()
        {
            base.OnClose();
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }


        public void OnClickAbility()
        {
            var abilityPanels = FindObjectsOfType<MineSceneUiAbilityButton>()?.ToList();

            if (abilityPanels == null)
            {
                App.Instance.Services.LogService.LogError("cant find ability panel");
            }
            else
            {
                var explosion = abilityPanels.Find(x => x.AbilityLevel != null && x.AbilityLevel.Type == AbilityType.ExplosiveStrike);
                if (explosion != null)
                {
                    explosion.Use();
                }
            }

            Close();

        }


        private void Subscribe()
        {

        }

        private void Unsubscribe()
        {
 
        }
    }
}