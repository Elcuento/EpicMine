using System;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowUpdate : WindowBase
    {
        [SerializeField] private GameObject _cancelButton;

        private bool _isCancelable;
        private Action _onClickOk;
        private Action _onClickCancel;


        public void Initialize(bool isCancelable, Action onClickOk = null, Action onClickCancel = null)
        {
            _isCancelable = isCancelable;

            _onClickOk = onClickOk;
            _onClickCancel = onClickCancel;

            _cancelButton.SetActive(_isCancelable);
        }


        public void Ok()
        {
            Close();
            _onClickOk?.Invoke();
        }

        public void Cancel()
        {
            if (!_isCancelable)
                return;

            Close();
            _onClickCancel?.Invoke();
        }
    }
}