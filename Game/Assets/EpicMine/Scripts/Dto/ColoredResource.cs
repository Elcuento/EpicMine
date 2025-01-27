namespace BlackTemple.EpicMine.Dto
{
    public struct ColoredResource
    {
        public string ResourceId { get; }

        public string Color { get; }

        public ColoredResource(string resourceId, string color)
        {
            ResourceId = resourceId;
            Color = color;
        }
    }
}