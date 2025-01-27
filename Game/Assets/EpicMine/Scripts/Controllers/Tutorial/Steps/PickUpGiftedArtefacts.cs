using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class PickUpGiftedArtefactsTutorialStep : TutorialStepBase
    {
        public PickUpGiftedArtefactsTutorialStep(bool isComplete) : base(TutorialStepIds.PickUpGiftedArtefacts, isComplete)
        {
            if (!IsComplete)
                Subscribe();
        }

        public override void CheckReady()
        {
            if (IsComplete)
                return;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ForceOpenChest))
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ScenesNames.Village)
                return;

            if (WindowManager.Instance.IsOpen<WindowOpenChest>())
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
            var window = WindowManager.Instance.Show<WindowDialogue>();
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_pick_up_gifted_artefacts");
            window.Initialize(dialogue, OnCloseWindowDialogue);
        }

        protected override void OnComplete()
        {
            Unsubscribe();

            var villageSceneController = Object.FindObjectOfType<VillageSceneController>();
            if (villageSceneController != null)
                villageSceneController.ScrollToBlacksmith();
        }


        private void OnSceneChange(string from, string to)
        {
            if (to != ScenesNames.Village)
                return;

            CheckReady();
        }

        private void OnCloseWindowDialogue()
        {
            OnGetArtefactsComplete();
        }

        private void OnGetArtefactsComplete()
        {
            var staticData = App.Instance.StaticData;

            var tierOpenCost = staticData.Configs.Dungeon.TierOpenArtefactsCost;

            var giftedArtefacts = tierOpenCost - App.Instance.Player.Artefacts.Amount;

            App.Instance.Player.Artefacts.Add(giftedArtefacts);

            var window = WindowManager.Instance.Show<WindowCustomGift>();
            var artefactsSprite = App.Instance.ReferencesTables.Sprites.ArtefactIcon;
            window.Initialize(artefactsSprite, giftedArtefacts, "tutorial_step_pick_up_gifted_artefacts_4", onClose: SetComplete);
        }

        private void OnWindowClose(WindowCloseEvent eventData)
        {
            if (eventData.Window is WindowOpenChest)
                CheckReady();
        }


        private void Subscribe()
        {
            SceneManager.Instance.OnSceneChange += OnSceneChange;
            EventManager.Instance.Subscribe<WindowCloseEvent>(OnWindowClose);
        }

        private void Unsubscribe()
        {
            if (SceneManager.Instance != null)
                SceneManager.Instance.OnSceneChange -= OnSceneChange;

            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<WindowCloseEvent>(OnWindowClose);
        }
    }
}