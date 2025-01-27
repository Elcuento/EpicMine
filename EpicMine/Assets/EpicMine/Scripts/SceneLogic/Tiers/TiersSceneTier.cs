using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable IdentifierTypo

namespace BlackTemple.EpicMine
{
    public class TiersSceneTier : MonoBehaviour
    {
        public Core.Tier Tier { get; private set; }

        public List<TiersSceneMine> Mines => _mines;

        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private List<TiersSceneMine> _mines;
        [SerializeField] private GameObject _openButton;
        [SerializeField] private Image _openButtonImage;
        [SerializeField] private GameObject _linesToNextTier;

        [SerializeField] private TextMeshProUGUI _artefactsCost;

        [SerializeField] private RectTransform _questIcon;

        private Action<TiersSceneTier, TiersSceneMine> _onMineClick;


        public void SetQuests(List<List<Quest>> quests)
        {
            var hasQuest = quests.Find(x => x.Count > 0) != null;
            _questIcon.gameObject.SetActive(hasQuest);

            for (var i = 0; i < Tier.Mines.Count; i++)
            {
                _mines[i].SetQuest(quests[i]);
            }
        }

        public void Initialize(Tier tier, Action<TiersSceneTier, TiersSceneMine> onMineClick)
        {
            Tier = tier;
            _onMineClick = onMineClick;

            _questIcon.DOAnchorPosY(-20, 2)
                .SetLoops(-1, LoopType.Yoyo);

            _artefactsCost.text = tier.StaticTier.RequireArtefacts.ToString();

            for (var i = 0; i < Tier.Mines.Count; i++)
            {
                var blMine = Tier.Mines[i];
                var mine = _mines[i];

                mine.Initialize(blMine, OnMineClick);
            }


            if (!tier.IsOpen)
            {
                _titleText.gameObject.SetActive(false);
                return;
            }

            SetOpen();
        }


        public void FadeIn()
        {
            _titleText.alpha = 1f;
            foreach (var mine in Mines)
                mine.FadeIn();
        }

        public void FadeOut()
        {
            _titleText.alpha = 0.5f;
            foreach (var mine in Mines)
                mine.FadeOut();
        }

        public void RemoveMineSelections()
        {
            foreach (var mine in Mines)
                mine.RemoveSelection();
        }

        public void ShowOpenButton()
        {
            _openButton.SetActive(true);
            _openButtonImage.sprite =
                App.Instance.Player.Artefacts.Amount >= MineHelper.GetTierRequireArtefacts(Tier.Number)
                    ? App.Instance.ReferencesTables.Sprites.ButtonGrown
                    : App.Instance.ReferencesTables.Sprites.ButtonGrey;
        }

        public void Open()
        {
            Tier.Open();
        }

        public void HideLinesToNextTier()
        {
            _linesToNextTier.SetActive(false);
        }

        private void Awake()
        {
            EventManager.Instance.Subscribe<TierOpenEvent>(OnTierOpen);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<TierOpenEvent>(OnTierOpen);
        }

        private void OnTierOpen(TierOpenEvent eventData)
        {
            if (eventData.Tier == Tier)
                SetOpen();
        }


        private void OnMineClick(TiersSceneMine mine)
        {
            _onMineClick(this, mine);
        }

        private void SetOpen()
        {
            _openButton.SetActive(false);
            _titleText.gameObject.SetActive(true);
            var tierName = LocalizationHelper.GetLocale("tier_" + Tier.Number);
            var tierLocale = LocalizationHelper.GetLocale("tier");
            _titleText.text = $"<color=#fff>{Tier.Number + 1} <lowercase>{tierLocale}</lowercase>:</color> {tierName}";
            _mines.First().SetOpen();
        }
    }
}