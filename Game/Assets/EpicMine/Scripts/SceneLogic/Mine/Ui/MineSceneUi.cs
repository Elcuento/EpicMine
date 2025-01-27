using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneUi : MonoBehaviour
    {
        public bool IsUiClickable { get; private set; }

        [SerializeField] private MineSceneUiWallInfoPanel _wallPanel;
        [SerializeField] private MineSceneUiAbilitiesPanel _abilityPanel;
        [SerializeField] private MineSceneUiItemsPanel _itemPanel;
        [SerializeField] private MineSceneUiProgressPanel _progressPanel;


        public void UpgradeButtonClick()
        {
            if (IsUiClickable)
                WindowManager.Instance.Show<WindowUpgrade>(withPause: true, withCurrencies: true);
        }

        public void PauseButtonClick()
        {
            if (IsUiClickable)
                WindowManager.Instance.Show<WindowMinePause>(withPause: true, withCurrencies: true);
        }

        public void PvpMinePauseButtonClick()
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
            EventManager.Instance.Subscribe<TierGhostAppearEvent>(OnGhostAppear);
            EventManager.Instance.Subscribe<TierGhostDisappearEvent>(OnGhostDisappear);
            EventManager.Instance.Subscribe<DialogStartedEvent>(OnDialogStartEvent);
            EventManager.Instance.Subscribe<DialogEndEvent>(OnDialogEndEvent);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
                EventManager.Instance.Unsubscribe<TierGhostAppearEvent>(OnGhostAppear);
                EventManager.Instance.Unsubscribe<TierGhostDisappearEvent>(OnGhostDisappear);
                EventManager.Instance.Unsubscribe<DialogStartedEvent>(OnDialogStartEvent);
                EventManager.Instance.Unsubscribe<DialogEndEvent>(OnDialogEndEvent);
            }
        }

        public void Hide()
        {
            _wallPanel.gameObject.SetActive(false);
            _abilityPanel.gameObject.SetActive(false);
            _itemPanel.gameObject.SetActive(false);
            _progressPanel.gameObject.SetActive(false);
        }

        public void Show()
        {
            _wallPanel.gameObject.SetActive(true);
            _abilityPanel.gameObject.SetActive(true);
            _itemPanel.gameObject.SetActive(true);
            _progressPanel.gameObject.SetActive(true);
        }

        private void OnGhostAppear(TierGhostAppearEvent eventData)
        {
            if (eventData.IsSpeak)
            {
                Hide();
            }else _wallPanel.gameObject.SetActive(false);
        }

        private void OnGhostDisappear(TierGhostDisappearEvent eventData)
        {
            Show();
        }

        private void OnDialogStartEvent(DialogStartedEvent eventData)
        {
            Hide();
        }
        private void OnDialogEndEvent(DialogEndEvent eventData)
        {
            Show();
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