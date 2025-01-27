using System;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowNotificationsNotification : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _shine;
        
        [SerializeField] private GameObject _chestPanel;
        [SerializeField] private GameObject _workshopSlotPanel;
        [SerializeField] private GameObject _artefactsPanel;
        [SerializeField] private GameObject _dailyTaskPanel;
        [SerializeField] private GameObject _autoMinerPanel;
        [SerializeField] private GameObject _questChangePanel;
        [SerializeField] private GameObject _questActivatePanel;
        [SerializeField] private GameObject _questStartPanel;

        [SerializeField] private Image _workshopSlotIcon;
        [SerializeField] private TextMeshProUGUI _dailyTaskTitle;

        private float _startPositionX = -700f;
        private float _endPositionX = 0f;
        private float _startShinePositionX = -100f;
        private float _endShinePositionX = 700f;

        
        public void Initialize(NotificationType notificationType)
        {
            Clear();

            switch (notificationType)
            {
                case NotificationType.ChestBreakingCompleted:
                    _chestPanel.SetActive(true);
                    break;
                case NotificationType.ArtefactsCollected:
                    _artefactsPanel.SetActive(true);
                    break;
                case NotificationType.AutoMinerFull:
                    _autoMinerPanel.SetActive(true);
                    break;
                case NotificationType.QuestChange:
                    _questChangePanel.SetActive(true);
                    break;
                case NotificationType.QuestActivate:
                    _questActivatePanel.SetActive(true);
                    break;
                case NotificationType.QuestStart:
                    _questStartPanel.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("notificationType", notificationType, null);
            }
            Show();
        }

        public void Initialize(Core.WorkshopSlot workshopSlot)
        {
            Clear();
            _workshopSlotPanel.SetActive(true);
            _workshopSlotIcon.sprite = SpriteHelper.GetIcon(workshopSlot.StaticRecipe.Id);
            Show();
        }

        public void Initialize(DailyTask dailyTask)
        {
            Clear();
            _dailyTaskPanel.SetActive(true);
            _dailyTaskTitle.text = LocalizationHelper.GetLocale(dailyTask.StaticTask.Id);
            Show();
        }


        private void Show()
        {
            _rectTransform
                .DOAnchorPosX(_endPositionX, 1f)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _shine.DOAnchorPosX(_endShinePositionX, 0.5f).SetUpdate(true);

                    _rectTransform
                        .DOAnchorPosX(_startPositionX, 1f)
                        .SetDelay(4f)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            Destroy(gameObject);
                        });
                });
        }

        private void Clear()
        {
            _rectTransform.anchoredPosition = new Vector2(_startPositionX, _rectTransform.anchoredPosition.y);
            _shine.anchoredPosition = new Vector2(_startShinePositionX, _shine.anchoredPosition.y);
            _chestPanel.SetActive(false);
            _workshopSlotPanel.SetActive(false);
            _workshopSlotIcon.sprite = null;
            _dailyTaskTitle.text = string.Empty;
        }
    }
}