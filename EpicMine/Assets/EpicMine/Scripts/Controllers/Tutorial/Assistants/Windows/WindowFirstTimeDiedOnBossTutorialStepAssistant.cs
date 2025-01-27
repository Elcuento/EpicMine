using System;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;

using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowFirstTimeDiedOnBossTutorialStepAssistant : WindowBase
    {
        [SerializeField] private GameObject _container0;
        [SerializeField] private GameObject _container1;

        [SerializeField] private Image _background;

        private bool _pickaxeDestroyed;
        private Action _onClose;


        public void Initialize(Action onClose)
        {
            _onClose = onClose;
            Clear();
            ShowButtonOpenUpgradeWindow();
        }

        public void ClickOpenUpgradeWindow()
        {
            Clear();
            WindowManager.Instance.Show<WindowUpgrade>(withCurrencies: true);
            ShowButtonUpgradeDamage();
        }

        public void ClickButtonUpgradeDamage()
        {
            App.Instance.Player.Skills.Damage.Up();
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
            _pickaxeDestroyed = WindowManager.Instance.IsOpen<WindowPickaxeDestroyed>();
            WindowManager.Instance.Close<WindowPickaxeDestroyed>();

            EventManager.Instance.Subscribe<SkillLevelChangeEvent>(OnCharacteristicLevelChange);
            EventManager.Instance.Subscribe<WindowCloseEvent>(OnCloseUpgradeWindow);

            if (!App.Instance.Player.Skills.Crit.CanUpgrade &&
                !App.Instance.Player.Skills.Damage.CanUpgrade &&
                !App.Instance.Player.Skills.Fortune.CanUpgrade)
            {
                _container1.SetActive(false);
                _background.raycastTarget = false;
            }
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

        private void Clear()
        {
            _container0.SetActive(false);
            _container1.SetActive(false);
            _background.raycastTarget = true;
        }

        public void OnCloseUpgradeWindow(WindowCloseEvent eventData)
        {
            if(eventData.Window is WindowUpgrade) 
                Complete();
        }

        private void Complete()
        {
            _onClose?.Invoke();
            Close();
   
            if (_pickaxeDestroyed)
                SceneManager.Instance.LoadScene(ScenesNames.Mine);
        }


        private void ShowButtonOpenUpgradeWindow()
        {
            _container0.SetActive(true);
        }

        private void ShowButtonUpgradeDamage()
        {
            _container1.SetActive(true);
        }

        private void OnCharacteristicLevelChange(SkillLevelChangeEvent eventData)
        {

          //  if (eventData.SkillLevel.Type != SkillType.Damage)
         //       return;

           // if (eventData.SkillLevel.Number >= TargetDamageLevel)
            {
               _container1.SetActive(false);
               _background.raycastTarget = false;
            }
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<SkillLevelChangeEvent>(OnCharacteristicLevelChange);
                EventManager.Instance.Unsubscribe<WindowCloseEvent>(OnCloseUpgradeWindow);
            }
        }
    }
}