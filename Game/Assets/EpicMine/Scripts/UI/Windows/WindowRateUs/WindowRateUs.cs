using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowRateUs : WindowBase
    {
        [SerializeField] private WindowRateUsStar[] _stars;
        [SerializeField] private Image _characterHeadImage;
        [SerializeField] private Sprite[] _characterHeadSprites;
        [SerializeField] private GameObject _rateButton;

        private int _starsCount;


        public void SetStars(int count)
        {
            _rateButton.SetActive(true);
            _starsCount = count;

            for (var i = 0; i < _stars.Length; i++)
            {
                var star = _stars[i];
                star.Initialize(i <= count - 1);
            }

            _characterHeadImage.sprite = _characterHeadSprites[count - 1];
        }

        public void Rate()
        {
            if (_starsCount >= 5)
                Application.OpenURL(UrlHelper.GetMarketUrl());
            else
            {
                var window = WindowManager.Instance.Show<WindowInformation>();
                window.Initialize("window_rate_us_thank_you_header", "window_rate_us_thank_you_description", "window_rate_us_thank_you_button");
            }

            Close();
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
            Clear();
            App.Instance.Services.AnalyticsService.CustomEvent("show_window_rate_us", new CustomEventParameters());
        }


        private void Clear()
        {
            _rateButton.SetActive(false);
            foreach (var star in _stars)
                star.Initialize(false);
        }
    }
}