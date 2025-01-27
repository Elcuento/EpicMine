using System;
using System.Collections;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowStartChestBreakingTutorialStepAssistant : WindowBase
    {
        [SerializeField] private Image _background;
        [SerializeField] private GameObject _container1;
        [SerializeField] private GameObject _container2;
        [SerializeField] private GameObject _container3;

        private Action _onClose;


        public void Initialize(Action onClose)
        {
            _onClose = onClose;
            StartCoroutine(ShowBurglarArrow());
            EventManager.Instance.Subscribe<ChestStartBreakingEvent>(OnChestStartBreaking);
        }

        public void ClickOnBurglar()
        {
            _container1.SetActive(false);
            _background.raycastTarget = false;

            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_start_breaking_chest");
            var windowDialogue = WindowManager.Instance.Show<WindowDialogue>();
            windowDialogue.Initialize(dialogue, OnCloseDialogue);
        }

        public void ClickOnChest()
        {
            var chests = FindObjectsOfType<VillageScenePlaceForChest>();
            var chest = chests.First(ch => ch.Chest != null);
            chest.Click();

            _container2.SetActive(false);
            _container3.SetActive(true);
        }

        public void ClickStartBreaking()
        {
            _container3.SetActive(false);
            var windowChestInfo = WindowManager.Instance.Get<WindowChestInfo>();
            windowChestInfo.StartBreaking();
        }


        private IEnumerator ShowBurglarArrow()
        {
            var villageSceneController = FindObjectOfType<VillageSceneController>();
            villageSceneController.BurglarButtonClick();

            yield return new WaitForSeconds(1f);

            _container1.SetActive(true);
        }

        private void OnCloseDialogue()
        {
            _container2.SetActive(true);
            _background.raycastTarget = true;
        }

        private void OnChestStartBreaking(ChestStartBreakingEvent eventData)
        {
            Unsubscribe();
            _onClose?.Invoke();
            WindowManager.Instance.Close(this, withSound: false);
        }


        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<ChestStartBreakingEvent>(OnChestStartBreaking);
            }
        }
    }
}