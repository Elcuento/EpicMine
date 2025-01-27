using BlackTemple.Common;
using System.Collections.Generic;
using CommonDLL.Dto;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Player = BlackTemple.EpicMine.Core.Player;

namespace BlackTemple.EpicMine
{
    public class WindowLeaderBoard : WindowBase
    {
        [SerializeField] private WindowLeaderBoardItem _itemPrefab;
        [SerializeField] private ScrollRect _usersScroll;
        [SerializeField] private GameObject _hint;

        [Space]
        [SerializeField] private Toggle _newBies;
        [SerializeField] private Toggle _miners;
        [SerializeField] private Toggle _pvp;

        [Space]
        [SerializeField] private TextMeshProUGUI _newBiesText;
        [SerializeField] private TextMeshProUGUI _minersText;
        [SerializeField] private TextMeshProUGUI _pvpText;

        [Space]
        [SerializeField] private Color _activeFilterTabColor;
        [SerializeField] private Color _inactiveFilterTabColor;

        [Space]
        private List<PlayerMineRating> _newBieLeaderBoard = new List<PlayerMineRating>();
        private List<PlayerMineRating> _leaderBoard = new List<PlayerMineRating>();
        private List<PlayerPvpRating> _pvpLeaderBoard = new List<PlayerPvpRating>();

        [Space]
        private List<WindowLeaderBoardItem> _items = new List<WindowLeaderBoardItem>();
 
        public void SetTab()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            _usersScroll.verticalNormalizedPosition = 1;
            SetFilterTabsColors();

            if (_miners.isOn) { InitializeLeaderBoard(); return; }
            if (_pvp.isOn) { InitializePvpBoard(); return; }
            if (_newBies.isOn) { InitializeNewbieBoard(); return; }

}
        private void SetFilterTabsColors()
        {
            _newBiesText.color = _newBies.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
            _minersText.color = _miners.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
            _pvpText.color = _pvp.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
        }

        private void DisableUnUsedItems(int from)
        {
            for(var i = from; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }
        }

        private void InitializeNewbieBoard()
        {
            for (var i = 0; i < _newBieLeaderBoard.Count; i++)
            {
                var user = _newBieLeaderBoard[i];
                var isPlayerItem = !string.IsNullOrEmpty(App.Instance.Player.Nickname) && user.UserNick.ToLower() == App.Instance.Player.Nickname.ToLower();

                if (_items.Count > i && _items.Count != 0)
                {
                    _items[i].gameObject.SetActive(true);
                    _items[i].Initialize(i, user, isPlayerItem);
                }
                else
                {
                    var item = Instantiate(_itemPrefab, _usersScroll.content, false);
                    item.Initialize(i, user, isPlayerItem);
                    _items.Add(item);
                }
            }
            DisableUnUsedItems(_newBieLeaderBoard.Count);
        }
        private void InitializeLeaderBoard()
        {
            for (var i = 0; i < _leaderBoard.Count; i++)
            {
                var user = _leaderBoard[i];
                var isPlayerItem = !string.IsNullOrEmpty(App.Instance.Player.Nickname) && user.UserNick.ToLower() == App.Instance.Player.Nickname.ToLower();

                if (_items.Count > i && _items.Count != 0)
                {
                    _items[i].gameObject.SetActive(true);
                    _items[i].Initialize(i, user, isPlayerItem);
                }
                else
                {
                    var item = Instantiate(_itemPrefab, _usersScroll.content, false);
                    item.Initialize(i, user, isPlayerItem);
                    _items.Add(item);
                }
            }
            DisableUnUsedItems(_leaderBoard.Count);
        }
        private void InitializePvpBoard()
        {
            
            for (var i = 0; i < _pvpLeaderBoard.Count; i++)
            {
                var user = _pvpLeaderBoard[i];
                var isPlayerItem = !string.IsNullOrEmpty(App.Instance.Player.Nickname) && user.UserNick.ToLower() == App.Instance.Player.Nickname.ToLower();
//
               // Debug.Log(user.UserNick +":" + user.Rating);
                if (_items.Count > i && _items.Count != 0)
                {
                    _items[i].gameObject.SetActive(true);
                    _items[i].Initialize(i, user, isPlayerItem, true);
                }
                else
                {
                    var item = Instantiate(_itemPrefab, _usersScroll.content, false);
                    item.Initialize(i, user, isPlayerItem, true);
                    _items.Add(item);
                }
            }
            DisableUnUsedItems(_pvpLeaderBoard.Count);
        }

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            App.Instance.Controllers.RatingsController.Refresh();

            _leaderBoard = App.Instance.Controllers.RatingsController.GetMineRating();
            _pvpLeaderBoard = App.Instance.Controllers.RatingsController.GetPvpRating();
            _newBieLeaderBoard = App.Instance.Controllers.RatingsController.GetNewbieMineRating();

            if (string.IsNullOrEmpty(App.Instance.Player.Nickname))
            {
                WindowManager.Instance.Show<WindowNickname>().Initialize(OnGetNicknameComplete);
            }

            SetFilterTabsColors();

            App.Instance.Controllers.RedDotsController.ViewLeaderBoard();
            InitializeNewbieBoard();


        }

        private void OnGetNicknameComplete()
        {
            ShowHint();
            OnShow();
        }

       /* private void Update()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                App.Instance.Controllers.RatingsController.AddTimeAndCalc();
                OnShow();
            }
        }*/
        public void ShowHint()
        {
            _hint.SetActive(true);
        }

        public void CloseHint()
        {
            _hint.SetActive(false);
        }

        public override void OnClose()
        {
            base.OnClose();
            _usersScroll.content.ClearChildObjects();
            _items.Clear();
        }
    }
}