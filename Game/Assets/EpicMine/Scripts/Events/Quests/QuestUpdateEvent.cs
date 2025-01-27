using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct QuestUpdateEvent
    {
        public Quest Quest;

        public QuestUpdateEvent(Quest quest)
        {
            Quest = quest;
        }
    }
}