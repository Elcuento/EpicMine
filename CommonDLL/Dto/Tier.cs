using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class Tier
    {
        public int Number;

        public bool IsOpen;

        public List<Mine> Mines;

        public List<string> UnlockedDropItems;

        public Tier(int number, bool isOpen)
        {
            Number = number;
            IsOpen = isOpen;
            Mines = new List<Mine>();
            UnlockedDropItems = new List<string>();
        }

        public Tier()
        {

        }
        public Tier(int number, bool isOpen, List<Mine> mines, List<string> unlockedDropItems)
        {
            Number = number;
            IsOpen = isOpen;
            Mines = mines;
            UnlockedDropItems = unlockedDropItems;
        }
    }
}