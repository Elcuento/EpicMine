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
    }
}