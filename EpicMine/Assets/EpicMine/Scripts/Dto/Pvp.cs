using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlackTemple.EpicMine.Dto
{
    public struct Pvp
    {
        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("games")]
        public int Games { get; set; }

        [JsonProperty("win")]
        public int Win { get; set; }

        [JsonProperty("loose")]
        public int Loose { get; set; }

        [JsonProperty("chests")]
        public int Chests { get; set; }

        [JsonProperty("inviteDisable")]
        public bool InviteDisable { get; set; }

        [JsonProperty("lastTimePlayed")]
        public List<LastTimePlayed> LastTimePlayed { get; set; }


        public Pvp(int rating, int win, int loose , int chests, int games, bool inviteDisable, List<LastTimePlayed> lti)
        {
            Rating = rating;
            Win = win;
            Chests = chests;
            Loose = loose;
            InviteDisable = inviteDisable;
            LastTimePlayed = lti;
            Games = games;
        }
    }
}