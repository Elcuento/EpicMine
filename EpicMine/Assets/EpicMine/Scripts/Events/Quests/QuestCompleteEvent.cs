using BlackTemple.EpicMine.Core;

namespace BlackTemple.EpicMine
{
    public struct QuestCompleteEvent
    {
        public Quest Quest;

        public QuestCompleteEvent(Quest quest)
        {
            Quest = quest;
        }
    }
}