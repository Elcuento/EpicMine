using System;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class TiersSceneMine : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _selectedBorder;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _locked;
        [SerializeField] private RatingView _rating;

        [SerializeField] private GameObject _questIcon;

        public Core.Mine Mine { get; private set; }

        private Action<TiersSceneMine> _onClick;


        public void Initialize(Mine mine, Action<TiersSceneMine> onClick)
        {
            Mine = mine;
            _onClick = onClick;

            if (Mine.IsOpen)
            {
                SetOpen();
            }
            else
            {
                _locked.SetActive(true);
                _rating.gameObject.SetActive(false);
                _icon.gameObject.SetActive(false);
            }
        }

        public void SetQuest(List<Quest> quests)
        {
            _questIcon.SetActive(quests.Count > 0);
        }

        public void FadeIn()
        {
            _canvasGroup.alpha = 1f;
        }

        public void FadeOut()
        {
            _canvasGroup.alpha = 0.5f;
        }

        public void RemoveSelection()
        {
            _selectedBorder.SetActive(false);
        }

        public void ToggleHardcore(bool isOn)
        {
            Mine.ToggleHardcore(isOn);
            _rating.gameObject.SetActive(Mine.IsComplete);
            _rating.Initialize(isOn ? Mine.HardcoreRating : Mine.Rating, isOn ? ViewRatingType.Skulls : ViewRatingType.Stars);
        }

        public void Click()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            if (!Mine.IsOpen)
                return;

            _onClick(this);
            _selectedBorder.SetActive(true);
        }

        public void SetOpen()
        {
            _locked.SetActive(false);
            _icon.gameObject.SetActive(true);

            if (Mine.IsComplete)
            {
                _rating.gameObject.SetActive(true);
                _icon.sprite = App.Instance.ReferencesTables.Sprites.MineCompleteIcon;

                _rating.Initialize(
                    Mine.IsHardcoreOn
                        ? Mine.HardcoreRating
                        : Mine.Rating,
                    Mine.IsHardcoreOn
                        ? ViewRatingType.Skulls
                        : ViewRatingType.Stars);

                return;
            }

            if (Mine.IsLast)
            {
                _icon.sprite = App.Instance.ReferencesTables.Sprites.MineLastIcon;
                return;
            }

            _icon.sprite = App.Instance.ReferencesTables.Sprites.MineIncompleteIcon;
        }
    }
}