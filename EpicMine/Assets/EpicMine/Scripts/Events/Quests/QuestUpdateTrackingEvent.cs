namespace BlackTemple.EpicMine
{
    public struct QuestUpdateTrackingEvent
    {
        public Core.Quest Quest;

        public QuestUpdateTrackingEvent(Core.Quest quest)
        {
            Quest = quest;
        }
    }
}