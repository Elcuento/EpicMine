using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowFade : WindowBase
    {
        [SerializeField] private Image _fadeImage;
        private WindowFadeSettings _defaultSettings;


        public void FadeIn()
        {
            _fadeImage.raycastTarget = true;
            _fadeImage.color = _defaultSettings.Color;
            _fadeImage.DOFade(1, _defaultSettings.Time)
                .SetUpdate(true);
        }

        public void FadeOut()
        {
            _fadeImage.color = _defaultSettings.Color;
            _fadeImage
                .DOFade(0, _defaultSettings.Time)
                .OnComplete(() => { _fadeImage.raycastTarget = false; })
                .SetUpdate(true);
        }


        public void FadeIn(WindowFadeSettings settings)
        {
            settings.Color.a = 0f;

            _fadeImage.raycastTarget = true;
            _fadeImage.color = settings.Color;
            _fadeImage.DOFade(1, settings.Time)
                .SetUpdate(true);
            
        }

        public void FadeOut(WindowFadeSettings settings)
        {
            _fadeImage.color = settings.Color;
            _fadeImage
                .DOFade(0, settings.Time)
                .OnComplete(() => { _fadeImage.raycastTarget = false; })
                .SetUpdate(true);
        }


        private void Start()
        {
            _defaultSettings = new WindowFadeSettings(0.4f, Color.black);
        }
    }
}