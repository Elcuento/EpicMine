using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdatePickaxe : SendData
    {
        public List<Pickaxe> Items;

        public RequestDataUpdatePickaxe(List<Pickaxe> items)
        {
            Items = items;
        }
    }
}