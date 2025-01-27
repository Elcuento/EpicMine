using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateCurrency : SendData
    {
        public List<Currency> Items;

        public RequestDataUpdateCurrency(List<Currency> items)
        {
            Items = items;
        }
    }
}