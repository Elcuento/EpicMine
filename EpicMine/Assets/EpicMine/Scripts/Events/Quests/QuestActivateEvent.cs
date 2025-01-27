using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct QuestActivateEvent
    {
        public Quest Quest;

        public QuestActivateEvent(Quest quest)
        {
            Quest = quest;
        }
    }
}