using System;
using System.Collections;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowCraftResourcesSecondPartTutorialStepAssistant : WindowBase
    {
        [SerializeField] private Image _background;
        [SerializeField] private GameObject _container0;
        [SerializeField] private GameObject _container1;
        [SerializeField] private GameObject _container2;

        private Action _onClose;
        private WindowWorkshopSlot _slot;

        private bool _gotSlot;

        public void Initialize(Action onClose)
        {
            _onClose = onClose;
            Clear();

            if (WindowManager.Instance.IsOpen<WindowWorkshop>())
                StartCoroutine(GetSlot());
            else
                _container0.SetActive(true);
        }


        public void ClickOpenWorkshop()
        {
            _container0.SetActive(false);
            WindowManager.Instance.Show<WindowWorkshop>();
            StartCoroutine(GetSlot());
        }

        public void ClickButtonForceComplete()
        {
            _slot.WorkshopSlot.ForceComplete(isTutorial: true);
        }

        public void ClickButtonCollectCompleted()
        {
            if (_slot.WorkshopSlot.StaticRecipe == null)
            {
                _onClose?.Invoke();
                Clear();
                WindowManager.Instance.Close(this, withSound: false);
            }
            else
            {
                _slot.CollectCompleted();
            }
        }


        private IEnumerator GetSlot()
        {
            if(_gotSlot ) yield break;

            while (_slot == null)
            {
                var slots = FindObjectsOfType<WindowWorkshopSlot>().ToList().OrderBy(x => x.WorkshopSlot.Number)
                    .ToList();


                if (slots.Capacity > 0)
                {
                    _slot = slots.First(s => s.WorkshopSlot.IsUnlocked);
                }
                

                yield return null;
            }

            if (_gotSlot) yield break;

            _gotSlot = true;
            _background.raycastTarget = false;
            var dialogue = App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_craft_resources_second_part");
            var windowDialogue = WindowManager.Instance.Show<WindowDialogue>();
            windowDialogue.Initialize(dialogue, OnCloseWindowDialogue);
        }

        private void OnCloseWindowDialogue()
        {
            _background.raycastTarget = true;
            Clear();

            if (_slot.WorkshopSlot.CompleteAmount != _slot.WorkshopSlot.NecessaryAmount)
                ShowButtonForceComplete();
            else
                ShowButtonCollectCompleted();
        }

        private void OnSlotComplete(WorkshopSlotCompleteEvent eventData)
        {
            if (WindowManager.Instance.IsOpen<WindowWorkshop>())
            {
                Clear();
                ShowButtonCollectCompleted();
            }
        }

        private void OnSlotClear(WorkshopSlotClearEvent eventData)
        {
            _onClose?.Invoke();
            Clear();
            WindowManager.Instance.Close(this, withSound: false);
        }

        private void OnWindowOpen(WindowOpenEvent eventData)
        {
            if (eventData.Window is WindowCollectWorkshopSlot)
            {
                _background.raycastTarget = false;
                HideButtonCollectCompleted();
            }
        }

        private void OnWindowClose(WindowCloseEvent eventData)
        {
            if (eventData.Window is WindowCollectWorkshopSlot)
            {
                _background.raycastTarget = true;
                ShowButtonCollectCompleted();
            }
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
            EventManager.Instance.Subscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
            EventManager.Instance.Subscribe<WorkshopSlotClearEvent>(OnSlotClear);
            EventManager.Instance.Subscribe<WindowOpenEvent>(OnWindowOpen);
            EventManager.Instance.Subscribe<WindowCloseEvent>(OnWindowClose);
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


        private void Update()
        {
            if (_slot != null)
                _slot.ForceCompleteButtonCostText.text = "0";
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
                EventManager.Instance.Unsubscribe<WorkshopSlotClearEvent>(OnSlotClear);
                EventManager.Instance.Unsubscribe<WindowOpenEvent>(OnWindowOpen);
                EventManager.Instance.Unsubscribe<WindowCloseEvent>(OnWindowClose);
            }
        }

        private void Clear()
        {
            _container1.SetActive(false);
            _container2.SetActive(false);
        }

        private void ShowButtonForceComplete()
        {
            _container1.SetActive(true);
        }

        private void ShowButtonCollectCompleted()
        {
            _container2.SetActive(true);
        }

        private void HideButtonCollectCompleted()
        {
            _container2.SetActive(false);
        }
    }
}