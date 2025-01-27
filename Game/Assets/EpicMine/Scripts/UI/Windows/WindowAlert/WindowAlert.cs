using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowAlert : WindowBase
    {
        [SerializeField] private CanvasGroup _alert;
        [SerializeField] private TextMeshProUGUI _text;

        public void Initialize(string text, float fadeTime = 3f, bool isNeedLocalizeText = true)
        {
            Clear();

            var locale = isNeedLocalizeText ? LocalizationHelper.GetLocale(text) : text;
            _text.text = locale;

            _alert.DOFade(0f, fadeTime).SetUpdate(true);
        }

        private void Clear()
        {
            _text.text = string.Empty;
            _alert.DOKill();
            _alert.DOFade(1f, 0f).SetUpdate(true);
        }
    }
}