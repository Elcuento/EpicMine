namespace CommonDLL.Static
{
    public class Monologue
    {
        public string Id ;

        public string DialogueId ;

        public string CharacterId ;

        public string StartAnimation ;

        public string CycleAnimation ;

        public string EndAnimation ;

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