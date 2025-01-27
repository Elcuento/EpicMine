using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class Recipe
    {
        public string Id;

        public List<string> FoundResources;

        public Recipe(string id, List<string> foundResources)
        {
            Id = id;
            FoundResources = foundResources;
        }

    }
}