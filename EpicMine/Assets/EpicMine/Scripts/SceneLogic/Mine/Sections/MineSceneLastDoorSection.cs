using System.Collections;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackTemple.EpicMine
{
    public class MineSceneLastDoorSection : MineSceneSection, IPointerDownHandler
    {
        [SerializeField] private Animator _animator;

        private const float OpenTime = 1f;

        private bool _isJustNowUnlocked;

        private bool _isNextTierLocked;

        private bool _isOpened;

        public override void Initialize(int number, MineSceneHero hero)
        {
            base.Initialize(number, hero);

            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            var nextClosedTier = App.Instance.Player.Dungeon.Tiers.FirstOrDefault(t => t.IsOpen == false);
            _isNextTierLocked = nextClosedTier != null && selectedTier.Number == nextClosedTier.Number - 1;

            if (!_isNextTierLocked)
                _animator.Play("Open");
        }

        public override void SetReady()
        {
            base.SetReady();
            if (!_isNextTierLocked)
                SetPassed();
        }

        protected override void SetPassed(float delay = MineLocalConfigs.OtherSectionMoveDelay)
        {
            base.SetPassed(delay);
            StartCoroutine(ShowWindow());
        }


        private IEnumerator OpenDoors()
        {
            _animator.Play("Open");
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.LastDoor);
            yield return new WaitForSeconds(OpenTime);
            _isJustNowUnlocked = true;
            SetPassed();
        }

        private IEnumerator ShowWindow()
        {
            yield return new WaitForSeconds(MineLocalConfigs.SectionMoveTime);
            if (!_isJustNowUnlocked)
                EventManager.Instance.Publish(new MineSceneEndGameEvent());
            else
            {
                MineHelper.ClearTempStorage();
                SceneManager.Instance.LoadScene(ScenesNames.Tiers);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isOpened)
                return;

            if (_isNextTierLocked)
            {
                _isOpened = true;
                var window = WindowManager.Instance.Show<WindowLastMineComplete>(withSound: false);
                window.Initialize(OnUnlockTier);
            }
        }

        private void OnUnlockTier()
        {
            StartCoroutine(OpenDoors());
        }
    }
}