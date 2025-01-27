using BlackTemple.EpicMine.Core;
using CommonDLL.Dto;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowLeaderBoardItem : MonoBehaviour
    {
        [SerializeField] private Color[] _flagsColors;

        [Space]
        [SerializeField] private Image _flagIcon;
        [SerializeField] private Image _locationFlagIcon;
        [SerializeField] private TextMeshProUGUI _number;
        [SerializeField] private TextMeshProUGUI _nickname;

        [Header("Icons")]
        [SerializeField] private Image _prestigeIcon;
        [SerializeField] private Image _ratingIcon;
        [SerializeField] private Image _pvpRatingIcon;
        [SerializeField] private Image _hardcoreRatingIcon;

        [Space]
        [SerializeField] private TextMeshProUGUI _progress;

        [Space]
        [SerializeField] private Image _background;
        [SerializeField] private Sprite _defaultBackgroundSprite;
        [SerializeField] private Sprite _playerBackgroundSprite;

        [Space]
        [SerializeField] private GameObject _indicators;
        [SerializeField] private Image _indicatorArrow;
        [SerializeField] private TextMeshProUGUI _indicatorText;
        [SerializeField] private Color _indicatorUpColor;
        [SerializeField] private Color _indicatorDownColor;

        [Space]
        [SerializeField] private TextMeshProUGUI _ratingText;
        [SerializeField] private TextMeshProUGUI _hardcoreRatingText;
        [SerializeField] private TextMeshProUGUI _pvpRatingText;


        public void Initialize(int index, PlayerPvpRating leaderBoardItem, bool isPlayerItem = false, bool isPvp = false)
        {
            if (index < _flagsColors.Length)
                _flagIcon.color = _flagsColors[index];

            _number.text = (index + 1).ToString();

            var pureName = leaderBoardItem.UserNick;

            if (leaderBoardItem.UserNick.Contains(PvpLocalConfig.BotNamePrefix))
            {
                pureName = leaderBoardItem.UserNick.Replace(PvpLocalConfig.BotNamePrefix, "");
            }

            var userData = App.Instance.Controllers.RatingsController.GetPlayer(leaderBoardItem.UserId);

            _nickname.text = pureName;
            _locationFlagIcon.sprite = SpriteHelper.GetFlagShortCode(string.IsNullOrEmpty(userData?.Location)
                ? "EN" : userData?.Location);

                _ratingIcon.gameObject.SetActive(false);
                _hardcoreRatingIcon.gameObject.SetActive(false);
                _pvpRatingIcon.gameObject.SetActive(true);

                _ratingText.text = "";
                _hardcoreRatingText.text = "";
                _pvpRatingText.text = ""; // TODO

            if (leaderBoardItem.Prestige > 0)
            {
                _prestigeIcon.gameObject.SetActive(true);
                _prestigeIcon.sprite = SpriteHelper.GetPrestigeIcon(leaderBoardItem.Prestige);
            }
            else
                _prestigeIcon.gameObject.SetActive(false);

            
                _progress.text = $"{LocalizationHelper.GetLocale("league_" + (PvpHelper.GetLeagueByRating(leaderBoardItem.Rating) + 1))}";
                _pvpRatingText.text = leaderBoardItem.Rating.ToString();

            _background.sprite = isPlayerItem
                ? _playerBackgroundSprite
                : _defaultBackgroundSprite;

            if (leaderBoardItem.Position != leaderBoardItem.OldPosition)
            {
                var isAbove = leaderBoardItem.Position < leaderBoardItem.OldPosition;
                _indicators.SetActive(true);

                var color = isAbove
                    ? _indicatorUpColor
                    : _indicatorDownColor;

                _indicatorArrow.color = color;
                _indicatorText.color = color;

                if (!isAbove)
                    _indicatorArrow.transform.Rotate(new Vector3(0, 0, 180f));

                _indicatorText.text = Mathf.Abs(leaderBoardItem.Position - leaderBoardItem.OldPosition).ToString();
            }
        }

        public void Initialize(int index, PlayerMineRating leaderBoardItem, bool isPlayerItem = false, bool isPvp = false)
        { 
            if (index < _flagsColors.Length)
                _flagIcon.color = _flagsColors[index];

            _number.text = (index + 1).ToString();

            var pureName = leaderBoardItem.UserNick;

            if (leaderBoardItem.UserNick.Contains(PvpLocalConfig.BotNamePrefix))
            {
                pureName = leaderBoardItem.UserNick.Replace(PvpLocalConfig.BotNamePrefix, "");
            }

            var userData = App.Instance.Controllers.RatingsController.GetPlayer(leaderBoardItem.UserId);

            _nickname.text = pureName;
            _locationFlagIcon.sprite = SpriteHelper.GetFlagShortCode(string.IsNullOrEmpty(userData?.Location)
                ? "EN" : userData?.Location);

            if (isPvp) {
                _ratingIcon.gameObject.SetActive(false);
                _hardcoreRatingIcon.gameObject.SetActive(false);
                _pvpRatingIcon.gameObject.SetActive(true);

                _ratingText.text = "";
                _hardcoreRatingText.text = "";
                _pvpRatingText.text = ""; // TODO
            }
            else {
                _ratingIcon.gameObject.SetActive(true);
                _hardcoreRatingIcon.gameObject.SetActive(true);
                _pvpRatingIcon.gameObject.SetActive(false);

                _ratingText.text = leaderBoardItem.Rating.ToString();
                _hardcoreRatingText.text = leaderBoardItem.HardCoreRating.ToString();
                _pvpRatingText.text = "";
            }


            if (leaderBoardItem.Prestige > 0)
            {
                _prestigeIcon.gameObject.SetActive(true);
                _prestigeIcon.sprite = SpriteHelper.GetPrestigeIcon(leaderBoardItem.Prestige);
            }
            else
                _prestigeIcon.gameObject.SetActive(false);

            if(!isPvp)
            { 
            var tierLocale = LocalizationHelper.GetLocale("tier");
            var mineLocale = LocalizationHelper.GetLocale("mine");
            _progress.text = $"{leaderBoardItem.Tier} {tierLocale}, {leaderBoardItem.Mine} {mineLocale}";
            }
            else
            {
                _progress.text =  $"{LocalizationHelper.GetLocale("league_"+ (PvpHelper.GetLeagueByRating(leaderBoardItem.Rating) + 1))}";
                _pvpRatingText.text = leaderBoardItem.Rating.ToString();
            }

            _background.sprite = isPlayerItem
                ? _playerBackgroundSprite
                : _defaultBackgroundSprite;

            if (leaderBoardItem.Position != leaderBoardItem.OldPosition)
            {
                var isAbove = leaderBoardItem.Position < leaderBoardItem.OldPosition;
                _indicators.SetActive(true);

                var color = isAbove
                    ? _indicatorUpColor
                    : _indicatorDownColor;

                _indicatorArrow.color = color;
                _indicatorText.color = color;

                if (!isAbove)
                    _indicatorArrow.transform.Rotate(new Vector3(0, 0, 180f));

                _indicatorText.text = Mathf.Abs(leaderBoardItem.Position - leaderBoardItem.OldPosition).ToString();
            }
        }
    }
}