namespace CommonDLL.Dto
{ 
    public class Item
    {
        public string Id;

        public int Amount;

        public Item(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}