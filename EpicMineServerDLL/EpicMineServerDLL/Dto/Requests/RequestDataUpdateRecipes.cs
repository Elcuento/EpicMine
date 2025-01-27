using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateRecipes : SendData
    {
        public List<Recipe> Items;

        public RequestDataUpdateRecipes(List<Recipe> items)
        {
            Items = items;
        }
    }
}