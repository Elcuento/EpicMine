using System;
using System.Collections;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowForceOpenChestTutorialStepAssistant : WindowBase
    {
        [SerializeField] private Image _background;
        [SerializeField] private GameObject _container1;
        [SerializeField] private GameObject _container2;

        private Action _onClose;
        private VillageScenePlaceForChest _chest;
        private WindowChestInfo _windowChestInfo;


        public void Initialize(Action onClose)
        {
            _onClose = onClose;

            var chests = FindObjectsOfType<VillageScenePlaceForChest>();
            _chest = chests.First(ch => ch.Chest != null);

            StartCoroutine(ShowChestArrow());
            EventManager.Instance.Subscribe<BurglarChestOpenedEvent>(OnChestOpened);
        }

        public void ClickOnChest()
        {
            _chest.Click();

            _container1.SetActive(false);
            _container2.SetActive(true);

            _windowChestInfo = WindowManager.Instance.Get<WindowChestInfo>();
        }

        public void ClickForceOpen()
        {
            _container2.SetActive(false);
            _windowChestInfo.ForceComplete(isTutorial: true);
        }


        private IEnumerator ShowChestArrow()
        {
            var villageSceneController = FindObjectOfType<VillageSceneController>();
            villageSceneController.BurglarButtonClick();

            yield return new WaitForSeconds(1f);

            _background.raycastTarget = false;
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_force_open_chest");
            var windowDialogue = WindowManager.Instance.Show<WindowDialogue>();
            windowDialogue.Initialize(dialogue, OnCloseDialogue);
        }

        private void OnCloseDialogue()
        {
            if (_chest.Chest.IsBroken)
            {
                Unsubscribe();
                _onClose?.Invoke();
                WindowManager.Instance.Close(this, withSound: false);
                return;
            }

            _container1.SetActive(true);
            _background.raycastTarget = true;
        }


        private void OnChestOpened(BurglarChestOpenedEvent eventData)
        {
            Unsubscribe();
            _windowChestInfo = null;
            _onClose?.Invoke();
            WindowManager.Instance.Close(this, withSound: false);
        }

        private void Update()
        {
            if (_windowChestInfo != null) 
                _windowChestInfo.BreakingPanelOpenNowCost.text = "0";

            if (_chest != null)
                _chest.ForceOpenCost.text = "0";
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<BurglarChestOpenedEvent>(OnChestOpened);
            }
        }
    }
}