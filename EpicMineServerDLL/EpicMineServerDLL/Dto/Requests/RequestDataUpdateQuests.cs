using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateQuests : SendData
    {
        public List<Quest> Items;

        public RequestDataUpdateQuests(List<Quest> items)
        {
            Items = items;
        }
    }
}