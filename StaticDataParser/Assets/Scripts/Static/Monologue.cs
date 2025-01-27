namespace BlackTemple.EpicMine.Static
{
    public class Monologue
    {
        public string Id { get; }

        public string DialogueId { get; }

        public string CharacterId { get; }

        public string StartAnimation { get; }

        public string CycleAnimation { get; }

        public string EndAnimation { get; }

        public Monologue(string id, string dialogueId, string characterId, string startAnimation, string cycleAnimation, string endAnimation)
        {
            Id = id.ToLower();
            DialogueId = dialogueId.ToLower();
            CharacterId = characterId.ToLower();
            StartAnimation = startAnimation;
            CycleAnimation = cycleAnimation;
            EndAnimation = endAnimation;
        }
    }
}