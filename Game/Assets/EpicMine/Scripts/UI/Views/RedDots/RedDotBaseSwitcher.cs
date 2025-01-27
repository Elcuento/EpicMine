using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class RedDotBaseSwitcher : MonoBehaviour
    {
        [SerializeField] private List<RedDotBaseView> _redDots;
        [SerializeField] private RedDotBaseView _redDot;

        public void Start()
        {
            foreach (var redDotBaseView in _redDots)
                redDotBaseView.OnChange += OnChange;
            
            StartCoroutine(UpdateCanvas());

            if (App.Instance.Controllers.TutorialController.IsComplete)
                return;

            EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnTutorialStepCompleteEvent);
            EventManager.Instance.Subscribe<TutorialStepReadyEvent>(OnTutorialReadyEvent);
        }

        private void OnTutorialReadyEvent(TutorialStepReadyEvent data)
        {
            StartCoroutine(UpdateCanvas());
        }

        private void OnTutorialStepCompleteEvent(TutorialStepCompleteEvent eventData)
        {
            StartCoroutine(UpdateCanvas());
        }

        public void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnTutorialStepCompleteEvent);
                EventManager.Instance.Unsubscribe<TutorialStepReadyEvent>(OnTutorialReadyEvent);
            }

            foreach (var redDotBaseView in _redDots)
              if(redDotBaseView != null)
                  redDotBaseView.OnChange -= OnChange;
            
        }

        public void OnChange(RedDotBaseView dot)
        {
            if (!gameObject.activeInHierarchy)
                return;

            StartCoroutine(UpdateCanvas());
        }

        private IEnumerator UpdateCanvas()
        {
            yield return new WaitForEndOfFrame();
            var count = _redDots.Count(x => x.IsActive && _redDot.gameObject.activeSelf);
            _redDot.Show(count);
        }
    }
}