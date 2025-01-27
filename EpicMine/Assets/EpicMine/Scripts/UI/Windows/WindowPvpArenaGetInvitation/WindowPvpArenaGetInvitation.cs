using System;
using System.Collections;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Player = BlackTemple.EpicMine.Core.Player;

namespace BlackTemple.EpicMine
{
    public class WindowPvpArenaGetInvitation : WindowBase {

        [SerializeField] private TextMeshProUGUI _inviterName;
        [SerializeField] private Image _inviterPickAxe;
        [SerializeField] private TextMeshProUGUI _inviterRating;
        [SerializeField] private TextMeshProUGUI _inviterLeague;

        [SerializeField] private Button _reciveInviteButton;
        [SerializeField] private Checkbox _inviteEnableCheckBox;

        public string InviteName { private set; get; }
        public string MatchId { private set; get; }
        public string InviteId { private set; get; }

        public void Start()
        {
            _inviteEnableCheckBox.SetOn(!App.Instance.Player.Pvp.InviteDisable,true);
            _inviteEnableCheckBox.OnChange += OnChangeInviteState;

        }

        public void TimeOut()
        {
            App.Instance.Player.Pvp.DeniedInvite();
            PvpArenaNetworkController.DestroyMatchData();
            Close();
        }

        public void OnDestroy()
        {
            _inviteEnableCheckBox.OnChange -= OnChangeInviteState;

            if (EventManager.Instance!=null)
            EventManager.Instance.Unsubscribe<PvpArenaGetCancelInviteEvent>(OnInviteCancelled);
        }

        public void OnInviteCancelled(PvpArenaGetCancelInviteEvent eventData)
        {
            PvpArenaNetworkController.DestroyMatchData();

            WindowManager.Instance.Show<WindowInformation>().Initialize("pvp_invite_denied", "pvp_invite_denied_disc",
                "window_place_for_chest_button");

            Close();
        }

        public void Initialize(PvpArenaUserInfo user, string matchId)
        {
           
            if (user == null || user.Id ==App.Instance.Player.Id)
                return;

            Clear();

              MatchId = matchId;
              InviteId = user.Id;
              InviteName = user.Name;
             _inviterName.text = user.Name;
             _inviterRating.text = user.Rating.ToString();
             _inviterLeague.text = LocalizationHelper.GetLocale("league_" + (PvpHelper.GetLeagueByRating(user.Rating) + 1));
             _inviterPickAxe.enabled = true;
             _inviterPickAxe.sprite = SpriteHelper.GetPickaxeImage(user.Pickaxe);

            EventManager.Instance.Subscribe<PvpArenaGetCancelInviteEvent>(OnInviteCancelled);


            /*PvpArenaNetworkController.CreateMatchData(new PvpArenaMatchInfo
            {
                RoomName = _invitorId,
            });*/

            Invoke("TimeOut", PvpLocalConfig.DuelTimeOut);
        }

        public void OnClickClose()
        {
            WindowManager.Instance.Close(this, true);

            App.Instance.Player.Pvp.DeniedInvite();
            PvpArenaNetworkController.DestroyMatchData();

            //TODO

        }

        public void OnClickAccept()
        {
            MineHelper.ClearTempStorage();
            _reciveInviteButton.interactable = false;

            App.Instance.Player.Pvp.SendAcceptInvite(InviteId, MatchId, () =>
            {
                WindowManager.Instance.Close(this, true);
            });
        }


        public override void OnClose()
        {
            base.OnClose();

            CancelInvoke("TimeOut");
            EventManager.Instance.Unsubscribe<PvpArenaGetCancelInviteEvent>(OnInviteCancelled);
        }

        public void Clear()
        {
            MatchId = "";
            InviteId = "";
            InviteName = "";
            _inviterName.text = "";
            _inviterRating.text = "";
            _inviterLeague.text = "";
            _inviterPickAxe.enabled = false;
            _reciveInviteButton.interactable = true;
            CancelInvoke("TimeOut");
            EventManager.Instance.Unsubscribe<PvpArenaGetCancelInviteEvent>(OnInviteCancelled);
        }

        public void OnChangeInviteState(bool state)
        {
            App.Instance.Player.Pvp.SetInviteState(!state);
        }
    }
}
