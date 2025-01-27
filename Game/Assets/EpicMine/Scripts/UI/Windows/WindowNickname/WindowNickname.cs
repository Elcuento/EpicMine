using System;
using BlackTemple.Common;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowNickname : WindowBase
    {
        public Action OnSuccesses;

        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private GameObject _invalidNickname;
        [SerializeField] private GameObject _nicknameAlreadyExist;
        [SerializeField] private GameObject _nicknameAlreadyExistIcon;
        [SerializeField] private GameObject _nicknameAvailable;
        [SerializeField] private GameObject _nicknameAvailableIcon;
        [SerializeField] private GameObject _hint1;
        [SerializeField] private GameObject _hint2;
        [SerializeField] private Image _sendButtonImage;

        private string[] _blockedNames = { };

        private bool _isNicknameSaved;
        private string _nickname;

        public void Save()
        {
            if (!_isNicknameSaved)
                return;

            Close();
        }

        public override void OnClose()
        {
            base.OnClose();
            if (!_isNicknameSaved)
                SceneManager.Instance.LoadScene(ScenesNames.Village);
            _inputField.text = string.Empty;
            CLear();
    
        }

        public void Initialize(Action onSuccesses)
        {
            OnSuccesses = onSuccesses;
        }

        private void Start()
        {
            _inputField.onEndEdit.AddListener(OnEndEdit);
            _inputField.onValueChanged.AddListener(OnValueChanged);
            CLear();
        }

        private void OnDestroy()
        {
            if (_inputField != null)
            {
                _inputField.onEndEdit.RemoveListener(OnEndEdit);
                _inputField.onValueChanged.RemoveListener(OnValueChanged);
            }
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies, withRating);
            _blockedNames = _blockedNames.Length == 0
                ? Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(App.Instance.ReferencesTables.FileData
                    .BlockedNames.text)
                : _blockedNames;

        }

        private void OnEndEdit(string nickname)
        {
            if (_isNicknameSaved)
                return;

            _nickname = nickname.Trim();
            var nicknameToCheck = _nickname.ToLower();

            if (nicknameToCheck.Length <= 0)
                return;

            if (!Extensions.CheckText(nicknameToCheck))
            {
                _invalidNickname.SetActive(true);
                _nicknameAlreadyExistIcon.SetActive(nicknameToCheck.Length > 0);
                return;
            }

            if (nicknameToCheck.Length <= 3)
            {
                _invalidNickname.SetActive(true);
                _nicknameAlreadyExistIcon.SetActive(nicknameToCheck.Length > 0);
                return;
            }

            foreach (var blockedName in _blockedNames)
            {
                if (nicknameToCheck.Contains(blockedName))
                {
                    _invalidNickname.SetActive(true);
                    _nicknameAlreadyExistIcon.SetActive(true);
                    return;
                }
            }

            App.Instance.Player.SetNickName(_nickname);
            App.Instance.Controllers.RatingsController.Refresh();

            App.Instance.Services.AnalyticsService.SetUserNickname(_nickname);

            _invalidNickname.SetActive(false);
            _nicknameAlreadyExist.SetActive(false);
            _nicknameAlreadyExistIcon.SetActive(false);
            _nicknameAvailable.SetActive(true);
            _nicknameAvailableIcon.SetActive(true);

            _hint1.SetActive(false);
            _hint2.SetActive(true);

            _isNicknameSaved = true;
            _sendButtonImage.sprite = App.Instance.ReferencesTables.Sprites.ButtonGreen;

            OnSuccesses?.Invoke();
        }

        private void OnValueChanged(string nickname)
        {
            CLear(false);
        }


        private void CLear(bool total = true)
        {
            _invalidNickname.SetActive(false);
            _nicknameAlreadyExist.SetActive(false);
            _nicknameAvailable.SetActive(false);
            _hint1.SetActive(true);
            _hint2.SetActive(false);
            _nicknameAlreadyExistIcon.SetActive(false);
            _nicknameAvailableIcon.SetActive(false);

            if(total)
            _isNicknameSaved = false;

            _sendButtonImage.sprite = App.Instance.ReferencesTables.Sprites.ButtonGrey;
        }
    }
}