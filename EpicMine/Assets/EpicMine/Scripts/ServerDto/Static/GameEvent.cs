using System.Collections.Generic;

namespace CommonDLL.Static
{
    public class GameEvent
    {
        public string Id ;

        public GameEventType Type ;

        public GameEventExpireType ExpireType;

        public List<string> Quests ;
    }
}