using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MinePvpSceneUi : MonoBehaviour
    {
        public bool IsUiClickable { get; private set; }

        [SerializeField] private MinePvpSceneUiWallInfoPanel _wallPanel;
        [SerializeField] private MineSceneUiAbilitiesPanel _abilityPanel;
        [SerializeField] private MinePvpSceneUiProgressPanel _progressPanel;


        public void UpgradeButtonClick()
        {
            if (IsUiClickable)
                WindowManager.Instance.Show<WindowUpgrade>(withPause: true, withCurrencies: true);
        }

        public void PauseButtonClick()
        {
            if (IsUiClickable)
                WindowManager.Instance.Show<WindowPvpMinePause>(withPause: false, withCurrencies: true);
        }

        public void InventoryButtonClick()
        {
            if (IsUiClickable)
                WindowManager.Instance.Show<WindowInventory>(withPause: true, withCurrencies: true);
        }

        private void Awake()
        {
            EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            }
        }

        public void Hide()
        {
            _wallPanel.gameObject.SetActive(false);
            _abilityPanel.gameObject.SetActive(false);
            _progressPanel.gameObject.SetActive(false);
        }

        public void Show()
        {
            _wallPanel.gameObject.SetActive(true);
            _abilityPanel.gameObject.SetActive(true);
            _progressPanel.gameObject.SetActive(true);
        }

        private void OnSectionReady(MineSceneSectionReadyEvent data)
        {
            IsUiClickable = true;
        }

        private void OnSectionPassed(MineSceneSectionPassedEvent data)
        {
            IsUiClickable = false;
        }
    }
}