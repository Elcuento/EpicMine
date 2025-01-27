using BlackTemple.Common;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public partial class WindowLearnMiningBasicTutorialStepAssistant : WindowBase
    {
        [SerializeField] private Camera _tutorialHightlightCameraPrefab;

        [SerializeField] private RectTransform _rootRectTransform;

        [SerializeField] private Image _background;

        [SerializeField] private RectTransform[] _popups;

        private State _currentState;

        private Camera _tutorialHighlightCamera;


        public void OnBackgroundClick()
        {
            if (_currentState == null)
                return;

            if (!_currentState.IsComplete)
                return;

            _currentState.GoToNextState();
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            var mainCameraTransform = Camera.main.transform;
            _tutorialHighlightCamera = Instantiate(_tutorialHightlightCameraPrefab, mainCameraTransform.position, mainCameraTransform.rotation);

            SetState(new AttackPointsPopupState(this));
            Subscribe();
        }

        public override void OnClose()
        {
            base.OnClose();
            Unsubscribe();
        }


        private void Update()
        {
            _currentState?.OnUpdate();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void SetState(State state)
        {
            _currentState?.OnExit();
            _currentState = state;
            _currentState?.OnEnter();
        }


        private void OnSectionHit(MineScenePickaxeHealthChangeEvent eventData)
        {
            if (eventData.Health >= App.Instance.StaticData.Configs.Dungeon.Mines.MaxHealth)
                return;

            /* var pickaxeHealthBarState = new PickaxeHealthBarPopupState(this);

             if (EventManager.Instance != null)
                 EventManager.Instance.Unsubscribe<MineScenePickaxeHealthChangeEvent>(OnSectionHit);

             if (_currentState is MiningResourcesPopupState)
             {
                 _currentState.SetNextState(pickaxeHealthBarState);
                 return;
             }

             SetState(pickaxeHealthBarState);*/
        }


        private void OnWallDamage(MineSceneWallSectionDamageEvent eventData)
        {
            if (eventData.HealthHandler.Health > 0)
                return;

            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<MineSceneWallSectionDamageEvent>(OnWallDamage);

         //   SetState(new MiningResourcesPopupState(this));
        }

        private void Subscribe()
        {
            EventManager.Instance.Subscribe<MineScenePickaxeHealthChangeEvent>(OnSectionHit);
            EventManager.Instance.Subscribe<MineSceneWallSectionDamageEvent>(OnWallDamage);
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineScenePickaxeHealthChangeEvent>(OnSectionHit);
                EventManager.Instance.Unsubscribe<MineSceneWallSectionDamageEvent>(OnWallDamage);
            }

            if (_tutorialHighlightCamera != null)
                Destroy(_tutorialHighlightCamera.gameObject);
        }
    }
}