namespace BlackTemple.EpicMine.Static
{
    public class Resource
    {
        public string Id { get; }

        public int Price { get; }

        public ResourceType Type { get; }

        public int? Filter { get; }

        public Resource(string id, int price, ResourceType type, int? filter)
        {
            Id = id;
            Price = price;
            Type = type;
            Filter = filter ?? 0;
        }
    }
}