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

        public Inventory(BlackTemple.EpicMine.Core.Inventory data)
        {
            Items = new List<Item>();

            foreach (var dataItem in data.Items)
            {
                Items.Add(new Item(dataItem.Id, dataItem.Amount));
            }
        }
    }
}