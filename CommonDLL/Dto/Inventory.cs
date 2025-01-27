using System.Collections.Generic;


namespace CommonDLL.Dto
{
    public class Inventory
    {
        public List<Item> Items;

        public Inventory()
        {
            Items =new List<Item>();
        }
    }
}