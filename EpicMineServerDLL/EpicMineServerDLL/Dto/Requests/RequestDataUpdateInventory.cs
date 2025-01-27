using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateInventory : SendData
    {
        public List<Item> Items;

        public RequestDataUpdateInventory(List<Item> items)
        {
            Items = items;
        }
    }
}