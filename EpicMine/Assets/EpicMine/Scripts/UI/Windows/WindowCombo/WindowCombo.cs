using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowCombo : WindowBase
    {
        [SerializeField] private Image[] _digits;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private RectTransform _container;
        [SerializeField] private CanvasGroup _containerGroup;

        private float _startPositionY = 220;
        private string _damageLocale;

        public void Initialize(int count)
        {
            Clear();

            if (string.IsNullOrEmpty(_damageLocale))
                _damageLocale = LocalizationHelper.GetLocale("damage");

            var digitIndex = Mathf.Clamp(count - 2, 0, _digits.Length - 1);
            var damageCoefficientIndex = Mathf.Clamp(count - 2, 0, App.Instance.StaticData.Configs.Dungeon.Mines.Combo.Count - 1);

            var digit = _digits[digitIndex];
            var damageCoefficient = App.Instance.StaticData.Configs.Dungeon.Mines.Combo[damageCoefficientIndex];

            _container.gameObject.SetActive(true);
            digit.gameObject.SetActive(true);
            digit.rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 1);
            _description.text = $"<lowercase>{_damageLocale}</lowercase> x{damageCoefficient}";

            _container.DOAnchorPosY(_container .anchoredPosition.y + 50, 1f).SetDelay(0.5f);
            _containerGroup.DOFade(0, 1f).SetDelay(0.5f);
        }

        //2 ярус третья шахта
        private void Clear()
        {
            foreach (var digit in _digits)
            {
                digit.DOKill(true);
                digit.rectTransform.DOKill(true);
                digit.rectTransform.localScale = Vector3.one;
                digit.gameObject.SetActive(false);
            }

            _container.DOKill(true);
            _container.anchoredPosition = new Vector2(0, _startPositionY);
            _container.gameObject.SetActive(false);
            _containerGroup.DOKill(true);
            _containerGroup.DOFade(1, 0);
            _description.text = string.Empty;
        }
    }
}