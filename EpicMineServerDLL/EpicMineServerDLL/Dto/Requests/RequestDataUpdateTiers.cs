using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateTiers : SendData
    {
        public List<Tier> Items;

        public RequestDataUpdateTiers(List<Tier> items)
        {
            Items = items;
        }
    }
}