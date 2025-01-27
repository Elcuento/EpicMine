using System.Collections.Generic;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateAdPickaxe : SendData
    {
        public Dictionary<string, int> Items;

        public RequestDataUpdateAdPickaxe(Dictionary<string, int> items)
        {
            Items = items;
        }
    }
}