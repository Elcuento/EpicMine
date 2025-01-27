namespace BlackTemple.EpicMine.Static
{
    public class Potion
    {
        public string Id { get; }

        public PotionType Type { get; }

        public int Time { get; }

        public float Value { get; }

        public Potion(string id, PotionType type, int time, float value)
        {
            Id = id;
            Type = type;
            Time = time;
            Value = value;
        }
    }
}