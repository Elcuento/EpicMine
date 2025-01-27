using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct QuestStartEvent
    {
        public Quest Quest;

        public QuestStartEvent(Quest quest)
        {
            Quest = quest;
        }
    }
}