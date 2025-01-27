using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class Pvp
    {
        public int Rating;
        public int Chests;

        public int Win;
        public int Loose;
        public int Games;
        public bool InviteDisable;
        public List<LastTimePlayed> LastTimePlayed;

        public Pvp()
        {

        }
        public Pvp(BlackTemple.EpicMine.Core.Pvp data)
        {
            Rating = data.Rating;
            Chests = data.Chests;
            Win = data.Win;
            Loose = data.Loose;
            Games = data.Games;
            InviteDisable = data.InviteDisable;
            LastTimePlayed = new List<LastTimePlayed>();
        }
    }
}