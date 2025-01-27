using System;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowPvpArenaSendInvitation : WindowBase {

    [SerializeField] private TextMeshProUGUI _inviterOnlineStatus;
    [SerializeField] private Image _inviterPickaxe;

    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject _warningIcon;
    [SerializeField] private GameObject _okIcon;

    [SerializeField] private GameObject _playerItemPrefab;
    [SerializeField] private Transform _playersTransform;

    [SerializeField] private TextMeshProUGUI _arenaName;
    [SerializeField] private Button _invitationButton;

    [SerializeField] private Color _onlinePlayerColor;
    [SerializeField] private Color _offlinePlayerColor;
    [SerializeField] private Color _playerNotFound;

    private string _invitePlayerId;


    private void Start()
    {
        _inputField.onEndEdit.AddListener(OnEndEdit);
        _inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDestroy()
    {
        if (_inputField != null)
        {
            _inputField.onEndEdit.RemoveListener(OnEndEdit);
            _inputField.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    private void OnEndEdit(string nickname)
    {
       var playerName = nickname.Trim().ToLower();

       var pure = playerName.Replace(PvpLocalConfig.BotNamePrefix.ToLower(), "");

       if (playerName.Length <= 3 || playerName.Length > 16 || playerName == App.Instance.Player.Id.ToLower() || !Extensions.CheckText(pure))
        {
            _warningIcon.SetActive(true);
            return;
        }

        if (!OnFindBot(playerName))
        {
           // App.Instance.Player.Pvp.FindUser(playerName, OnFind, OnFailed);
        }
    }

    public bool OnFindBot(string playerName)
    {
        if (playerName.Contains(PvpLocalConfig.BotNamePrefix.ToLower()))
        {
            var botName = playerName.Replace(PvpLocalConfig.BotNamePrefix.ToLower(), "");

            PvpBot bot = null;

            foreach (var botData in App.Instance.StaticData.PvpBots)
            {
                foreach (var s in botData.Names)
                {
                    if (s.ToLower() == botName)
                    {
                        bot = botData;
                        break;
                    }
                }
            }

         /*  if (bot != null)
            {
                OnFind(new ResponseDataPvpFindUserByName(bot.Id, botName, PvpBotHelper.GetRandomPickAxe(bot).ToString(), PvpBotHelper.GetRandomRating(bot), 0));
            }
            return true;*/
        }

        return false;
    }

  /*  public void OnFind(string id, string nameb,string pick, int rating, long timeOnline)
    {
        var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var lastTime = timeOnline;

        _invitePlayerId = id;
        _inviterPickaxe.enabled = true;
        _inviterPickaxe.sprite = SpriteHelper.GetPickaxeImage(string.IsNullOrEmpty(data.Pickaxe) ?
            App.Instance.StaticData.Pickaxes.First().Id : data.Pickaxe);

        var isOnline = timeStamp - lastTime < 60;

        SetStatusText(LocalizationHelper.GetLocale(isOnline ? "user_online" : "user_offline"), isOnline ? _onlinePlayerColor : _offlinePlayerColor);
        _invitationButton.interactable = isOnline;
    }*/

    public void SetStatusText(string text, Color col)
    {
        _inviterOnlineStatus.text = text;
        _inviterOnlineStatus.color = col;
    }
    public void OnFailed()
    {
        SetStatusText(LocalizationHelper.GetLocale("user_not_find"), _playerNotFound);
        _okIcon.SetActive(false);
        _warningIcon.SetActive(true);
    }

    private void OnValueChanged(string nickname)
    {
       ClearInputField();
    }

    public void ClearInputField()
    {
        _warningIcon.SetActive(false);
        _okIcon.SetActive(false);
        SetStatusText("", Color.green);
        _invitePlayerId = "";
        _inviterPickaxe.enabled = false;
        _invitationButton.interactable = false;
    }

    public void Initialize()
    {
        Clear();

        FillPlayers();
        _arenaName.text = LocalizationHelper.GetLocale("leagueArena_" + (PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating) + 1));
    }

    public void FillPlayers()
    {
        var ltp = App.Instance.Player.Pvp.LastTimePlayed;

        if (ltp == null)
            return;

        foreach (var player in ltp)
        {
            if(string.IsNullOrEmpty(player.PlayerName))
                continue;

            CreatePlayerItem(player.PlayerName);
        }
    }

    private void CreatePlayerItem(string playerName)
    {
        var item = Instantiate(_playerItemPrefab, _playersTransform, false);
        var pName = playerName;
        item.GetComponentInChildren<TextMeshProUGUI>().text = pName.Replace(PvpLocalConfig.BotNamePrefix, "");
        item.GetComponentInChildren<Button>().onClick.AddListener(() => { OnClickPlayer(pName); });
    }

    public void OnClickPlayer(string pName)
    {
        _inputField.text = pName.Replace(PvpLocalConfig.BotNamePrefix, "");
        OnEndEdit(pName);
    }

    public void OnClickInvitePlayer()
    {
        if (string.IsNullOrEmpty(_invitePlayerId))
            return;

        /*PvpArenaNetworkController.Create(null,() =>
        {
            Close();

            SceneManager.Instance.LoadScene(ScenesNames.PvpArena);

            App.Instance.Player.Pvp.SendInvite(_invitePlayerId);
        });*/
      
    }

    public void Clear()
    {
        _playersTransform.ClearChildObjects();
        _inputField.text = "";
        _invitePlayerId = "";
         SetStatusText("", Color.green);
        _inviterPickaxe.enabled = false;
        _warningIcon.SetActive(false);
        _invitationButton.interactable = false;
        _okIcon.SetActive(false);
    }
}
