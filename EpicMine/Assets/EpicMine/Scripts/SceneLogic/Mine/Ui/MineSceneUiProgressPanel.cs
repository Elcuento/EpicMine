using System.Collections;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiProgressPanel : MonoBehaviour
    {
        [SerializeField] private MineSceneController _sceneController;

        [Space] [SerializeField] private RectTransform _panel;

        [SerializeField] private TextMeshProUGUI _mineName;

        [SerializeField] private Slider _slider;

        [SerializeField] private Image _mineIcon;

        [SerializeField] private RatingView _rating;

        private const int SectionSize = 60;

        private const int BorderSize = 16;

        private Core.Mine _mine;

        private Core.Tier _tier;

        private bool _isHardcore;


        private void Awake()
        {
            _tier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            _mine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            _isHardcore = App.Instance
                .Services
                .RuntimeStorage
                .Load<bool>(RuntimeStorageKeys.IsHardcoreMode);

            if (_mine.IsLast)
            {
                _mineIcon.gameObject.SetActive(true);
                var nextClosedTier = App.Instance.Player.Dungeon.Tiers.FirstOrDefault(t => t.IsOpen == false);
                var isClosed = nextClosedTier != null && _tier.Number == nextClosedTier.Number - 1;
                _mineIcon.sprite = isClosed
                    ? App.Instance.ReferencesTables.Sprites.MineLastIcon
                    : App.Instance.ReferencesTables.Sprites.MineCompleteIcon;
            }
            else
            {
                if (_isHardcore)
                {
                    _rating.gameObject.SetActive(true);
                }
                else
                {
                    _mineIcon.gameObject.SetActive(true);
                    _mineIcon.sprite = _mine.IsComplete
                        ? App.Instance.ReferencesTables.Sprites.MineCompleteIcon
                        : App.Instance.ReferencesTables.Sprites.MineIncompleteIcon;
                }
            }

            Subscribe();
        }

        private void Start()
        {
            var tierLocale = LocalizationHelper.GetLocale("tier_" + _tier.Number);
            var mineLocale = LocalizationHelper.GetLocale("mine");

            _mineName.text = $"{tierLocale}: <nobr>{_mine.Number + 1} <lowercase>{mineLocale}</lowercase></nobr>";
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }


        private void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            if (!_mine.IsLast && !_mine.IsComplete)
            {
                if (eventData.Section.Number >= MineLocalConfigs.DefaultSectionsCount)
                    _panel.gameObject.SetActive(false);
            }

            UpdatePanelSize();
            UpdateRating();

            var progress = GetProgress(_sceneController.Hero.CurrentSection == null ? 0 : _sceneController.Hero.CurrentSection.Number, false);

            _slider.DOKill();
            _slider.DOValue(progress, 0f);
        }

        private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            StartCoroutine(UpdateProgress(eventData.Delay));
        }


        private void UpdatePanelSize()
        {
            int panelSize;
            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting) && !_isHardcore)
                panelSize = MineLocalConfigs.BlacksmithSectionPositionNumber + _mine.Number;
            else if (_tier.IsLast && _mine.IsLast)
                panelSize = MineLocalConfigs.GodSectionPositionNumber;
            else
            {
                if (_mine.IsLast || !_isHardcore)
                    panelSize = _mine.IsLast ? MineLocalConfigs.BossSectionsCount : MineLocalConfigs.DefaultSectionsCount;
                else
                {
                    var sectionsCountForRatingOne = RatingHelper.GetSectionsCountForHardcoreRating(1);
                    var sectionsCountForRatingTwo = RatingHelper.GetSectionsCountForHardcoreRating(2);
                    var sectionsCountForRatingThree = RatingHelper.GetSectionsCountForHardcoreRating(3);

                    if ((_sceneController.Hero.CurrentSection != null ? _sceneController.Hero.CurrentSection.Number : 0) + 1 <= sectionsCountForRatingOne)
                        panelSize = sectionsCountForRatingOne;
                    else if ((_sceneController.Hero.CurrentSection != null ? _sceneController.Hero.CurrentSection.Number : 0) + 1 <= sectionsCountForRatingOne + sectionsCountForRatingTwo)
                        panelSize = sectionsCountForRatingTwo;
                    else
                        panelSize = sectionsCountForRatingThree;
                }
            }

            _panel.DOSizeDelta(new Vector2(SectionSize * panelSize + BorderSize, _panel.sizeDelta.y), 0.3f);
        }

        private void UpdateRating()
        {
            var currentProgressRating = RatingHelper.GetMineHardcoreRating(_sceneController.Hero.CurrentSection == null ? 0 : _sceneController.Hero.CurrentSection.Number);

            var ratingType = _isHardcore
                ? ViewRatingType.Skulls
                : ViewRatingType.Stars;

            var currentRating = _isHardcore
                ? _mine.HardcoreRating
                : _mine.Rating;

            if (currentProgressRating < currentRating)
                currentProgressRating = currentRating;

            _rating.Initialize(currentProgressRating, ratingType);
        }

        private IEnumerator UpdateProgress(float delay = 0f)
        {
            yield return new WaitForSeconds(delay);

            var emptySectionsCount = 0;
            var nextSectionNumber = 0;

            var sections = _sceneController.SectionProvider.Sections.Where(s => s.Number > (_sceneController.Hero.CurrentSection != null ? _sceneController.Hero.CurrentSection.Number : 0)).ToList();
            if (sections.Count > 0)
            {
                nextSectionNumber = sections[0].Number;
            }

            var progress = GetProgress(nextSectionNumber, true);
            var moveTime = MineLocalConfigs.SectionMoveTime;

            _slider.DOKill();
            _slider.DOValue(progress, moveTime)
                .SetEase(Ease.Linear);
        }

        private float GetProgress(int sectionNumber, bool isNextRating)
        {
            if (_mine == null)
                return 1f;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting) && !_isHardcore)
                return sectionNumber / ((float)MineLocalConfigs.BlacksmithSectionPositionNumber + _mine.Number);

            if (_mine.IsLast && _tier.IsLast)
                return sectionNumber / (float)MineLocalConfigs.GodSectionPositionNumber;

           if (_mine.IsLast)
               return sectionNumber / (float)(MineLocalConfigs.BossSectionsCount + 1);

            if (!_isHardcore)
                return sectionNumber / (float)MineLocalConfigs.DefaultSectionsCount;

            var sectionsCountForRatingOne = RatingHelper.GetSectionsCountForHardcoreRating(1);
            var sectionsCountForRatingTwo = RatingHelper.GetSectionsCountForHardcoreRating(2);
            var sectionsCountForRatingThree = RatingHelper.GetSectionsCountForHardcoreRating(3);

            if (sectionNumber < sectionsCountForRatingOne)
                return sectionNumber / (float)sectionsCountForRatingOne;

            if (sectionNumber == sectionsCountForRatingOne)
            {
                if (isNextRating)
                    return sectionNumber / (float)sectionsCountForRatingOne;

                return 0f;
            }

            sectionNumber -= sectionsCountForRatingOne;
            if (sectionNumber < sectionsCountForRatingTwo)
            {
                if (isNextRating && sectionNumber == 0)
                    return 1f;

                return sectionNumber / (float)sectionsCountForRatingTwo;
            }

            if (sectionNumber == sectionsCountForRatingTwo)
            {
                if (isNextRating)
                    return sectionNumber / (float)sectionsCountForRatingTwo;

                return 0f;
            }


            sectionNumber -= sectionsCountForRatingTwo;
            if (sectionNumber < sectionsCountForRatingThree)
            {
                if (isNextRating && sectionNumber == 0)
                    return 1f;

                return sectionNumber / (float)sectionsCountForRatingThree;
            }

            return 1f;
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            }
        }
    }
}