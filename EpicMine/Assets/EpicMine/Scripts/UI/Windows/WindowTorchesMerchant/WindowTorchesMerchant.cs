using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class WindowTorchesMerchant : WindowBase
    {
        public List<WindowTorchesMerchantTorch> Torches { get; private set; }

        [SerializeField] private WindowTorchesMerchantTorch _torchPrefab;
        [SerializeField] private WindowTorchesMerchantItem _torchHeaderPrefab;
        [SerializeField] private GameObject _torchListPrefab;

        [SerializeField] private Transform _torchContainer;
        [SerializeField] private WindowTorchesMerchantTorchInfo _torchInfo;

        [Space]
        [SerializeField] private ScrollRect _torchesScrollRect;

        [Space]
        [SerializeField] private int _loadedLeague;

        private float _soundTime;
        private bool _scrollBusy;

        protected override void Awake()
        {
            base.Awake();
            Torches = new List<WindowTorchesMerchantTorch>();
        }

        public void Start()
        {
            EventManager.Instance.Subscribe<TorchCreateEvent>(OnTorchCreated);
            EventManager.Instance.Subscribe<TorchSelectEvent>(OnTorchSelected);
            EventManager.Instance.Subscribe<AdTorchesChangeEvent>(OnAdTorchesChange);

            Ready();
        }

        private void Clear()
        {
            _torchesScrollRect.content.ClearChildObjects();
            _loadedLeague = 0;
            Torches.Clear();
        }

        private void Update()
        {
            if (_soundTime > 0)
            {
                _soundTime -= Time.deltaTime;
                return;
            }

            _soundTime = Random.Range(3f, 8f);
            var randomSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.BlacksmithSounds.Length);
            var randomSound = App.Instance.ReferencesTables.Sounds.BlacksmithSounds[randomSoundIndex];
            AudioManager.Instance.PlaySound(randomSound);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public void OnScroll()
        {
            if (_scrollBusy || !IsReady) return;

            if (_torchesScrollRect.verticalNormalizedPosition >= 1 && _loadedLeague < 12)
            {
                _loadedLeague++;
                FillTorchesForSpecificLeague(_loadedLeague);
            }

        }

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies, true);

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            var selectedTorchLeague = App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.LeagueId;

            FillLowerLeagueTorches(selectedTorchLeague + 3);

            ScrollTo(selectedTorchLeague - 1, true);
            _torchInfo.ReInitialize();


            App.Instance.Controllers.RedDotsController.ViewTorchesWindow();
        }

        public void FillLowerLeagueTorches(int current)
        {
            if (_loadedLeague >= current - 1)
                return;

            current = current > 12 ? 12 : current;
            _loadedLeague = current;


            for (var i = 1; i <= current; i++)
                FillTorchesForSpecificLeague(i);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_torchesScrollRect.content);
            _torchesScrollRect.verticalNormalizedPosition = 0;
        }

        private void ScrollTo(int league, bool instat = false)
        {
            var leagueItem = _torchContainer.GetChild(league * 2).GetComponent<RectTransform>();

            StartCoroutine(ScrollToCoroutine(leagueItem, instat));
        }

        private IEnumerator ScrollToCoroutine(RectTransform target, bool immediately = false)
        {

            yield return new WaitUntil(() => IsReady || immediately);

            _scrollBusy = true;

            yield return new WaitForEndOfFrame();

            _torchesScrollRect.content.DOKill();

            var position = -target.anchoredPosition.y;
            var distance = Mathf.Abs(_torchesScrollRect.content.anchoredPosition.y - position);
            var duration = Mathf.Clamp01(distance / 4);

            _torchesScrollRect
                .content
                .DOAnchorPosY(-position, immediately ? 0f : duration)
                .OnComplete(() => { _scrollBusy = false; })
                .SetUpdate(true);
        }

        public void FillTorchesForSpecificLeague(int league)
        {
            var torches =
                App.Instance.Player.TorchesMerchant.Torches.FindAll(x => (x.StaticTorch.LeagueId == league && x.StaticTorch.Type != TorchType.Reward) 
                                                                         || (x.StaticTorch.Type == TorchType.Reward && x.StaticTorch.LeagueId == league && x.IsCreated));

            var torchBackground = Instantiate(_torchHeaderPrefab, _torchContainer, false);
            torchBackground.Initialize(league, LocalizationHelper.GetLocale("league_" + league), torches.Count);

            var torchList = Instantiate(_torchListPrefab, _torchContainer, false);

            torchList.transform.SetAsFirstSibling();
            torchBackground.transform.SetAsFirstSibling();

            foreach (var torch in torches)
            {
                var torchItem = Instantiate(_torchPrefab, torchList.transform, false);

                torchItem.Initialize(torch, OnTorchClick);
                Torches.Add(torchItem);
                if (torch.StaticTorch.Id == App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.Id)
                    _torchInfo.Initialize(torch);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(torchList.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(_torchesScrollRect.content);


        }


        private void Unsubscribe()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<TorchCreateEvent>(OnTorchCreated);
            EventManager.Instance.Unsubscribe<TorchSelectEvent>(OnTorchSelected);
            EventManager.Instance.Unsubscribe<AdTorchesChangeEvent>(OnAdTorchesChange);
        }

        private void OnTorchClick(WindowTorchesMerchantTorch torch)
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            foreach (var torchItem in Torches)
                torchItem.RemoveActive();

            torch.SetActive();
            torch.HideRedDot();
            _torchInfo.Initialize(torch.Torch);

            App.Instance.Controllers.RedDotsController.ViewPickaxe(torch.Torch.StaticTorch.Id);
        }


        private void OnRedDotTorchesMerchantChange(List<string> unlockedPickaxes)
        {
            var blacksmithCount = 0;
            var mythicalCount = 0;

            foreach (var torch in Torches)
            {
                torch.HideRedDot();
                if (unlockedPickaxes.Contains(torch.Torch.StaticTorch.Id))
                {
                    torch.ShowRedDot();
                }

            }
        }

        private void OnTorchCreated(TorchCreateEvent eventData)
        {
            if (!gameObject.activeSelf && eventData.Torch.StaticTorch.Type == TorchType.Reward)
            {
                Clear();

              //  var selectedTorchLeague = App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.LeagueId;

              //  FillLowerLeagueTorches(selectedTorchLeague + 3);
              //
              //  _torchInfo.ReInitialize();
              return;
            }

            foreach (var torch in Torches)
            {
                if (torch.Torch != eventData.Torch)
                    continue;

                torch.SetCreated();
                break;
            }

        }

        private void OnAdTorchesChange(AdTorchesChangeEvent eventData)
        {
            foreach (var i in eventData.AdTorches)
            {
                foreach (var torchItem in Torches)
                {
                    if (torchItem.Torch.StaticTorch.Type != TorchType.Ad)
                        continue;

                    if (torchItem.Torch.StaticTorch.Id != i.Key)
                        continue;

                    torchItem.UpdateAdVal(i.Value);
                }
            }

            _torchInfo.ReInitialize();
        }

        private void OnTorchSelected(TorchSelectEvent eventData)
        {
            foreach (var torch in Torches)
            {
                if (torch.Torch == eventData.Torch)
                    torch.SetSelected();
                else
                {
                    torch.RemoveSelected();
                }


            }
        }

    }
}