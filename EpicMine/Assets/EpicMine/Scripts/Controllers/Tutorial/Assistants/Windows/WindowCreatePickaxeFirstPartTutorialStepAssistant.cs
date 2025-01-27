using System;
using BlackTemple.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowCreatePickaxeFirstPartTutorialStepAssistant : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _bubbleText;
        [SerializeField] private Image _background;
        [SerializeField] private GameObject _blacksmith;
        [SerializeField] private GameObject _arrow;

        private Action _onClose;
        private int _currentIndex;


        public void Initialize(Action onClose)
        {
            _onClose = onClose;
            ShowFirstPopup();
        }

        public void ClickBackground()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            switch (_currentIndex)
            {
                case 0:
                    ShowSecondPopup();
                    break;
                case 1:
                    ShowThirdPopup();
                    break;
                case 3:
                    ShowArrow();
                    break;
            }
        }

        public void ClickCloseBlacksmithWindow()
        {
            if (_currentIndex == 4)
            {
                _onClose?.Invoke();
                WindowManager.Instance.Close<WindowBlacksmith>();
                WindowManager.Instance.Close(this, withSound: false);
            }
        }

        private void ShowFirstPopup()
        {
            _currentIndex = 0;
            _bubbleText.text = LocalizationHelper.GetLocale("tutorial_step_create_pickaxe_first_part_2");
        }

        private void ShowSecondPopup()
        {
            _currentIndex = 1;
            _bubbleText.text = LocalizationHelper.GetLocale("tutorial_step_create_pickaxe_first_part_3");
        }

        private void ShowThirdPopup()
        {
            _currentIndex = 3;
            _bubbleText.text = LocalizationHelper.GetLocale("tutorial_step_create_pickaxe_first_part_5");
        }

        private void ShowArrow()
        {
            _currentIndex = 4;
            _blacksmith.SetActive(false);
            _arrow.SetActive(true);
        }
    }
}