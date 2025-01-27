using System;

namespace CommonDLL.Dto
{
    public class Gifts
    {
        public long LastOpenTime;
        public int OpenedCount;

        public Gifts()
        {

        }
        public Gifts(BlackTemple.EpicMine.Core.Gifts data)
        {
            LastOpenTime = data.LastOpenTime;
            OpenedCount = data.OpenedCount;
        }
    }
}