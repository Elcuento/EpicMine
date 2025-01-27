namespace BlackTemple.EpicMine.Static
{
    public class QuestTaskGoalCollect
    {
        public string Id { get; private set; }

        public int Chance { get; }

        public QuestTaskGoalCollectSourceType SourceType;

        public string Source;

        public QuestTaskGoalCollect(string id, int chance, QuestTaskGoalCollectSourceType sourceType, string source)
        {
            Id = id;
            Chance = chance;
            SourceType = sourceType;
            Source = source;
        }
    }
}