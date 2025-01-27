using System.Collections.Generic;
using BlackTemple.Common;

namespace BlackTemple.EpicMine.Static
{
    public class GameEvent
    {
        public string Id;

        public GameEventType Type;

        public GameEventExpireType? ExpireType;

        public List<string> Quests;

        public GameEvent(string id, GameEventType type, GameEventExpireType? expireType, string quests)
        {
            Id = id;
            Type = type;
            ExpireType = expireType ?? GameEventExpireType.None;
            Quests = Extensions.SplitToList<string>(quests, "#");
        }
    }
}