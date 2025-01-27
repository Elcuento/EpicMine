using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowCreatePickaxeSecondPartTutorialStepAssistant : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _bubbleText;
        [SerializeField] private GameObject _blacksmith;
        [SerializeField] private GameObject _container1;
        [SerializeField] private GameObject _container2;

        private Action _onClose;
        private WindowBlacksmithPickaxe _pickaxe;
        private WindowBlacksmithPickaxeInfo _info;


        public void Initialize(Action onClose)
        {
            _onClose = onClose;
            StartCoroutine(GetPickaxe());
        }


        public void ClickOnPickaxe()
        {
            _pickaxe.Click();

            _blacksmith.SetActive(false);
            _container1.SetActive(false);
            _container2.SetActive(true);
        }

        public void ClickCreateOrChoosePickaxe()
        {
            if (_pickaxe.Pickaxe.IsCreated)
            {
                _info.Select();
                _onClose?.Invoke();
                WindowManager.Instance.Close(this, withSound: false);
            }
            else
                _info.Create();
        }


        private IEnumerator GetPickaxe()
        {
            var windowBlacksmith = WindowManager.Instance.Get<WindowBlacksmith>();

            while (_pickaxe == null)
            {
                if(windowBlacksmith.Pickaxes != null && windowBlacksmith.Pickaxes.Count >= 2)
                    _pickaxe = windowBlacksmith.Pickaxes[1];

                yield return null;
            }
            
            _info = FindObjectOfType<WindowBlacksmithPickaxeInfo>();
            _bubbleText.text = LocalizationHelper.GetLocale("tutorial_step_create_pickaxe_second_part_1");
            _blacksmith.SetActive(true);
            _container1.SetActive(true);
        }
    }
}