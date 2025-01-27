
using CommonDLL.Static;
using Newtonsoft.Json;
// ReSharper disable IdentifierTypo

namespace BlackTemple.EpicMine.Dto
{
    public struct Invite
    {
        [JsonProperty("inviterId")]
        public string InviterId;

        [JsonProperty("playerId")]
        public string PlayerId;

        [JsonProperty("playerName")]
        public string PlayerName;

        [JsonProperty("playerRating")]
        public int PlayerRating;

        [JsonProperty("playerPickaxe")]
        public string PlayerPickaxe;
        
        [JsonProperty("date")]
        public long Date;

        [JsonProperty("inviteStatus")]
        public PvpInviteStatusType InviteStatus;


        public Invite(string invitedId, string playerId, string playerName, int playerRating, string playerPickaxe, long date, int inviteStatus)
        {
            InviterId = invitedId;
            PlayerId = playerId;
            PlayerName = playerName;
            PlayerRating = playerRating;
            PlayerPickaxe = playerPickaxe;
            Date = date;
            InviteStatus = (PvpInviteStatusType) inviteStatus;
        }
    }
}