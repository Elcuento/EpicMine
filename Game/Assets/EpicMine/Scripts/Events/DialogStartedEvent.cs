namespace BlackTemple.EpicMine
{
    public struct DialogStartedEvent
    {
        public string Id;

        public DialogStartedEvent(string id)
        {
            Id = id;
        }
    }
}