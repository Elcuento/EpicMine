using System;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowShowCharacteristicsTutorialStepAssistant : WindowBase
    {
        [SerializeField] private GameObject _arrow;
        [SerializeField] private GameObject _blocker;
        [SerializeField] private GameObject _button;

        private Action _onClose;

        public void Initialize(Action onClose)
        {
            _onClose = onClose;
            EventManager.Instance.Subscribe<WindowOpenEvent>(OnWindowOpen);
            EventManager.Instance.Subscribe<WindowCloseEvent>(OnWindowClose);
        }

        private void OnWindowOpen(WindowOpenEvent eventData)
        {
            if (eventData.Window is WindowUpgrade)
            {
                _arrow.SetActive(false);
                _blocker.SetActive(false);
                _button.SetActive(false);
            
            }
        }

        private void OnWindowClose(WindowCloseEvent eventData)
        {
            if (eventData.Window is WindowUpgrade)
            {
                Unsubscribe();
                _onClose?.Invoke();
                WindowManager.Instance.Close(this, withSound: false);
            }
        }


        public void OnClick()
        {
            WindowManager.Instance.Show<WindowUpgrade>();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<WindowOpenEvent>(OnWindowOpen);
                EventManager.Instance.Unsubscribe<WindowCloseEvent>(OnWindowClose);
            }
        }
    }
}