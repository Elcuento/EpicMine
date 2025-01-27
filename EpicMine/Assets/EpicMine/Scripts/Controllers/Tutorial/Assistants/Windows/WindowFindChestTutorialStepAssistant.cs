using System;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowFindChestTutorialStepAssistant : WindowBase
    {
        [SerializeField] private Animator _arrow;

        private Action _onClose;
        private WindowMineChest _window;


        public void Initialize(Action onClose)
        {
            _onClose = onClose;
            _arrow.updateMode = AnimatorUpdateMode.UnscaledTime;
            _window = WindowManager.Instance.Get<WindowMineChest>();
            EventManager.Instance.Subscribe<WindowCloseEvent>(OnWindowClose);
        }

        public void SendToVillageClick()
        {
            _window.SendToVillage();
        }


        private void OnWindowClose(WindowCloseEvent eventData)
        {
            if (eventData.Window is WindowMineChest)
            {
                Unsubscribe();
                _onClose?.Invoke();
                WindowManager.Instance.Close(this, withSound: false);
            }
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<WindowCloseEvent>(OnWindowClose);
            }
        }
    }
}