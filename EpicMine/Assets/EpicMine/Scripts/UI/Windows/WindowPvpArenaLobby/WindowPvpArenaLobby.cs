using System.Collections;
using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowPvpArenaLobby : WindowBase
    {
        private PvpArenaNetworkController _arenaNetworkContoller;

        [SerializeField] private ScrollRect _arenaScrollList;
        [SerializeField] private SlideHandler SlideDragHandler;

        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private LobbyPvpArenaPreviewView _previewPrefab;

        [SerializeField] private PvpHeaderPanel _headerPanel;
        [SerializeField] private TextMeshProUGUI _league;

        [SerializeField] private TextMeshProUGUI _chests;
        [SerializeField] private Image _chestFill;

        [SerializeField] private TextMeshProUGUI _rating;
        [SerializeField] private TextMeshProUGUI _games;
        [SerializeField] private TextMeshProUGUI _winToLoose;

        [SerializeField] private GameObject _chestGetPanel;
        [SerializeField] private GameObject _chestCollectPanel;

        [SerializeField] private Checkbox _inviteCheckBox;

        [Space]
        [SerializeField] private Button _startButton;
        [SerializeField] private TextMeshProUGUI _startButtonLabel;
        [SerializeField] private GameObject _startButtonLoadingArrow;

        private int _chosenArena;
        private bool _arenaScrollReady;

        public void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<PvpArenaStartGameLobbyEvent>(OnGameLobby);
                EventManager.Instance.Unsubscribe<PvpUpdateChangeEvent>(OnPvpDataUpdate);
                EventManager.Instance.Unsubscribe<PvpArenaOnConnectedEvent>(OnConnected);
                EventManager.Instance.Unsubscribe<PvpArenaOnDisconnectedEvent>(OnDisconnected);
            }
            _inviteCheckBox.OnChange -= OnChangeInviteState;

        }

        protected override void Awake()
        {
            EventManager.Instance.Subscribe<PvpArenaStartGameLobbyEvent>(OnGameLobby);
            EventManager.Instance.Subscribe<PvpArenaStartGameEvent>(OnGameStarted);
            EventManager.Instance.Subscribe<PvpUpdateChangeEvent>(OnPvpDataUpdate);
            EventManager.Instance.Subscribe<PvpArenaOnConnectedEvent>(OnConnected);
            EventManager.Instance.Subscribe<PvpArenaOnDisconnectedEvent>(OnDisconnected);
            base.Awake();
        }

        public void Start()
        {
            _inviteCheckBox.SetOn(!App.Instance.Player.Pvp.InviteDisable, true);

            _inviteCheckBox.OnChange += OnChangeInviteState;

            SlideDragHandler.OnSliderDragLeft += OnSlideLeft;
            SlideDragHandler.OnSliderDragRight += OnSlideRight;

            _startButton.interactable = false;

            Ready();

            _chosenArena = PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating);
            ScrollTo(_chosenArena, true);

            if(!_arenaNetworkContoller.IsConnected)
                SetStartButtonLoading();
            else SetStartButtonDefault();


        }

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            if (string.IsNullOrEmpty(App.Instance.Player.Nickname))
            {
                var win = WindowManager.Instance.Show<WindowNickname>();
                win.Initialize(OnGetNicknameComplete);
            }

            App.Instance.Controllers.RedDotsController.ViewPvpWindow();
            
        }

        private void OnDisconnected(PvpArenaOnDisconnectedEvent eventData)
        {
            _startButton.interactable = false;
            SetStartButtonLoading();
        }
        private void OnConnected(PvpArenaOnConnectedEvent eventData)
        {
            _startButton.interactable = PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating) == _chosenArena;
            SetStartButtonDefault();
        }
        
        private void OnGetNicknameComplete()
        {
            _headerPanel.Initialize(PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating), App.Instance.Player.Nickname);
        }

        public void OnClickExit()
        {
            PvpArenaNetworkController.I.Leave(() =>
            {
                SceneManager.Instance.LoadScene(ScenesNames.Village);
            });

        }
        
        public void OnClickOpenInvitationWindow()
        {
            WindowManager.Instance.Show<WindowPvpArenaSendInvitation>()
                .Initialize();
        }

        public void OnClickGetChest()
        {
            var selectedTier = App.Instance.Player.Dungeon.LastOpenedTier.Number;

            WindowManager.Instance.Show<WindowOpenPvpChest>(withPause: false, withCurrencies: true)
                .Initialize(PvpChestType.Winner, selectedTier, false, UpdateChests);

            // window.Initialize(selectedTier, ChestType.Royal, UpdateChests);
        }
        public void OnClickNext()
        {
            if (_chosenArena < 11) _chosenArena++;
            ScrollTo(_chosenArena);
        }

        public void ScrollTo(int index, bool immediately = false)
        {
            var content = _arenaScrollList.content;
            StartCoroutine(ScrollTo(content.GetChild(index)
                .GetComponent<RectTransform>(), index, immediately));
        }

        public void OnClickPrev()
        {
            if (_chosenArena > 0) _chosenArena--;
            ScrollTo(_chosenArena);
        }

        public void UpdateChests()
        {
            _chestCollectPanel.SetActive(App.Instance.Player.Pvp.Chests != PvpLocalConfig.PvpWinChestRequire);
            _chestGetPanel.SetActive(App.Instance.Player.Pvp.Chests == PvpLocalConfig.PvpWinChestRequire);
        }

        public void Initialize(PvpArenaNetworkController photon)
        {
            _arenaNetworkContoller = photon;

            _headerPanel.Initialize(PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating), App.Instance.Player.Nickname);

            _league.text = LocalizationHelper.GetLocale("league_" + (PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating) + 1));
            _rating.text = App.Instance.Player.Pvp.Rating.ToString();
            _games.text = App.Instance.Player.Pvp.Games.ToString();
            _winToLoose.text = $"{App.Instance.Player.Pvp.Win} - {App.Instance.Player.Pvp.Loose}";
            _chestFill.fillAmount = App.Instance.Player.Pvp.Chests / (float)PvpLocalConfig.PvpWinChestRequire;
            _chests.text = $"{App.Instance.Player.Pvp.Chests}/{PvpLocalConfig.PvpWinChestRequire}";

            UpdateChests();

            var index = 0;

            foreach (var staticDataLeague in App.Instance.StaticData.Leagues)
            {
                var item = Instantiate(_previewPrefab, _arenaScrollList.content, false);
                item.Initialize(
                    SpriteHelper.GetArenaPreview(index + 1),
                    staticDataLeague.Rating.ToString(),
                    (index + 1).ToString(),
                    PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating) < index);

                index++;
            }


            LayoutRebuilder.ForceRebuildLayoutImmediate(_arenaScrollList.content);
        }


        public void OnChangeInviteState(bool state)
        {
            App.Instance.Player.Pvp.SetInviteState(!state);
        }

        public void OnPvpDataUpdate(PvpUpdateChangeEvent ev)
        {
            _league.text = LocalizationHelper.GetLocale("league_" + (PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating) + 1));
            _rating.text = App.Instance.Player.Pvp.Rating.ToString();
            _chestFill.fillAmount = App.Instance.Player.Pvp.Chests / (float)PvpLocalConfig.PvpWinChestRequire;
            _chests.text = $"{App.Instance.Player.Pvp.Chests}/{PvpLocalConfig.PvpWinChestRequire}";
            _games.text = App.Instance.Player.Pvp.Games.ToString();
            _winToLoose.text = $"{App.Instance.Player.Pvp.Win} - {App.Instance.Player.Pvp.Loose}";
        }

        public void OnGameStarted(PvpArenaStartGameEvent data)
        {
            Close();
        }

        public void OnGameLobby(PvpArenaStartGameLobbyEvent data)
        {
            Close();
        }

        public void OnSlideRight()
        {
            if (!_arenaScrollReady)
                return;

            OnClickNext();
        }
        public void OnSlideLeft()
        {
            if (!_arenaScrollReady)
                return;

            OnClickPrev();
        }

        private IEnumerator ScrollTo(RectTransform target, int index, bool immediately = false)
        {
            yield return new WaitUntil(()=> IsReady || immediately);

            _arenaScrollReady = false;

            yield return new WaitForEndOfFrame();

            _arenaScrollList.content.DOKill();

            var position = -target.anchoredPosition.x;
            var distance = Mathf.Abs(_arenaScrollList.content.anchoredPosition.x - position);
            var duration = Mathf.Clamp01(distance / 4);

            var avaliable = PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating) == index;
            SetFade(index, avaliable, immediately ? 0 : 1);

            _arenaScrollList
                .content
                .DOAnchorPosX(position, immediately ? 0f : duration)
                .OnComplete(() => 
                {
                    _startButton.interactable = avaliable && _arenaNetworkContoller.IsConnected;
                    _arenaScrollReady = true;
                })
                .SetUpdate(true);
        }

        public void SetFade(int index, bool centerInclude = false, float speed = 1)
        {
            var content = _arenaScrollList.content;
            if (index > 0)
            {
                content.GetChild(index - 1).gameObject.Fade(0.3f, speed);
            }

            if (index < _arenaScrollList.content.childCount - 1)
            {
                content.GetChild(index + 1).gameObject.Fade(0.3f, speed);
            }
            content.GetChild(index).gameObject.Fade(centerInclude ? 1 : 0.3f, speed);
        }


        public void OnClickStartFight()
        {
            if (PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating) != _chosenArena)
                return;

            StopAllCoroutines();
            StartCoroutine(ClickStartCoroutine());
        }


        public void StartRandomMatch()
        {
            _arenaNetworkContoller.JoinCreateRandom(_chosenArena, () =>
            {

            });
        }
        

        public IEnumerator ClickStartCoroutine()
        {
            SetStartButtonLoading();

            while (!_arenaNetworkContoller.IsConnected)
            {
                yield return new WaitForSeconds(1);
            }

            StartRandomMatch();

        }

        private void SetStartButtonLoading()
        {
            _startButtonLabel.gameObject.SetActive(false);
            _startButtonLoadingArrow.gameObject.SetActive(true);
        }

        private void SetStartButtonDefault()
        {
            _startButtonLabel.gameObject.SetActive(true);
            _startButtonLoadingArrow.gameObject.SetActive(false);

        }
    }
}