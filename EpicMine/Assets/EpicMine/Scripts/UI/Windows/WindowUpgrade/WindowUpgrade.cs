using System;
using System.Collections;
using System.Linq;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowUpgrade : WindowBase
    {
        [SerializeField] private WindowUpgradeSkillsPanel _skillsPanel;

        [SerializeField] private WindowUpgradeAbilitiesPanel _abilitiesPanel;

        [SerializeField] private Toggle _skillsTab;

        [SerializeField] private Toggle _abilityTab;

        [SerializeField] private TextMeshProUGUI _skillsTabText;

        [SerializeField] private TextMeshProUGUI _abilitiesTabText;

        [SerializeField] private Color _activeTabColor;

        [SerializeField] private Color _inactiveTabColor;

        [SerializeField] private GameObject _popup;

        [SerializeField] private VerticalLayoutGroup _popupVerticalLayoutGroup;

        [SerializeField] private TextMeshProUGUI _popupText;

        [SerializeField] private GameObject _skillsRedDot;

        [SerializeField] private GameObject _abilitiesRedDot;

        public void Start()
        {
            App.Instance.Controllers.RedDotsController.OnSkillsChange += OnSkillsChange;
            App.Instance.Controllers.RedDotsController.OnAbilitiesChange += OnAbilitiesChange;

            _abilityTab.interactable = App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.FirstEnergyAbility);
        }

        public void OnDestroy()
        {
            if (App.Instance == null)
                return;

            App.Instance.Controllers.RedDotsController.OnSkillsChange -= OnSkillsChange;
            App.Instance.Controllers.RedDotsController.OnAbilitiesChange -= OnAbilitiesChange;
        }

        public void CheckAbilitiesSkillsDots()
        {
            _abilitiesRedDot.SetActive(App.Instance.Controllers.RedDotsController.AbilitiesUpgradeAble.Find(x=> !x.Viewed) != null);
            _skillsRedDot.SetActive(App.Instance.Controllers.RedDotsController.SkillsUpgradeAble.Find(x => !x.Viewed) != null);
        }

        private void OnSkillsChange(bool isViewed)
        {
            _skillsRedDot.SetActive(!isViewed);
        }

        private void OnAbilitiesChange(bool isViewed)
        {
            _abilitiesRedDot.SetActive(!isViewed);

        }
        public void TogglePanel()
        {
            Clear();

            _skillsTabText.color = _skillsTab.isOn
                ? _activeTabColor
                : _inactiveTabColor;

            _abilitiesTabText.color = _skillsTab.isOn
                ? _inactiveTabColor
                : _activeTabColor;

            if (_skillsTab.isOn)
            {
                _skillsPanel.gameObject.SetActive(true);
                App.Instance.Controllers.RedDotsController.SetSkillsUpgradeViewed(true);
                _skillsPanel.Initialize();
                return;
            }

            _abilitiesPanel.gameObject.SetActive(true);
            App.Instance.Controllers.RedDotsController.SetAbilitiesUpgradeViewed(true);
            _abilitiesPanel.Initialize();
        }

        public void ShowPopup(string text)
        {
            _popupText.text = text;
            _popup.SetActive(true);
            StartCoroutine(FixPopupSize());
        }

        public void ClosePopup()
        {
            _popupText.text = string.Empty;
            _popup.SetActive(false);
        }

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
            TogglePanel();
            CheckAbilitiesSkillsDots();
            App.Instance.Controllers.RedDotsController.SetSkillsAbilitiesViewed(true);
        }


        private void Clear()
        {
            _skillsPanel.gameObject.SetActive(false);
            _abilitiesPanel.gameObject.SetActive(false);
            _skillsTabText.color = _inactiveTabColor;
            _abilitiesTabText.color =  _inactiveTabColor;
        }

        private IEnumerator FixPopupSize()
        {
            _popupVerticalLayoutGroup.enabled = false;
            yield return new WaitForEndOfFrame();
            _popupVerticalLayoutGroup.enabled = true;
        }
    }
}