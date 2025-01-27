using System.Collections.Generic;
using System.Linq;


namespace CommonDLL.Dto
{
    public class Dungeon
    {
        public List<Tier> Tiers;
        public Tier LastOpenedTier { get { return Tiers.LastOrDefault(t => t.IsOpen); } }
    }
}