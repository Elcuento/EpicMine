using CommonDLL.Static;

namespace CommonDLL.Dto

{
    public struct Invite
    {
        public string InviterId;

        public string PlayerId;

        public string PlayerName;

        public int PlayerRating;

        public string PlayerPickaxe;
        
        public long Date;

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