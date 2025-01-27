namespace BlackTemple.EpicMine.Static
{
    public class Dialogue
    {
        public string Id { get; }

        public Dialogue(string id)
        {
            Id = id.ToLower();
        }
    }
}