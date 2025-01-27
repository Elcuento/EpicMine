using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateTorches : SendData
    {
        public List<Torch> Items;

        public RequestDataUpdateTorches(List<Torch> items)
        {
            Items = items;
        }
    }
}