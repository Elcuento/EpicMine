using System.Collections.Generic;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateAdTorches : SendData
    {
        public Dictionary<string, int> Items;

        public RequestDataUpdateAdTorches(Dictionary<string, int> items)
        {
            Items = items;
        }
    }
}