using System.Collections;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackTemple.EpicMine
{
    public class MineSceneDoorSection : MineSceneSection, IPointerDownHandler
    {
        [SerializeField] private Animator _animator;

        private const float OpenTime = 1f;

        private Core.Mine _selectedMine;


        public override void Initialize(int number, MineSceneHero hero)
        {
            base.Initialize(number, hero);

            _selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (_selectedMine.IsComplete)
                _animator.Play("Open");
        }

        public override void SetReady()
        {
            base.SetReady();
            if (_selectedMine.IsComplete)
                SetPassed();
        }

        protected override void SetPassed(float delay = 0)
        {
            base.SetPassed(delay);
            StartCoroutine(ShowWindow());
        }


        private IEnumerator OpenDoors()
        {
            _animator.Play("Open");
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Door);
            yield return new WaitForSeconds(OpenTime);
            SetPassed();
        }

        private IEnumerator ShowWindow()
        {
            yield return new WaitForSeconds(MineLocalConfigs.SectionMoveTime);
            EventManager.Instance.Publish(new MineSceneEndGameEvent());
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_selectedMine.IsComplete)
                StartCoroutine(OpenDoors());
        }
    }
}