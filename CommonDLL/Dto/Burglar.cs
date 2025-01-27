using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class Burglar
    {
        public List<Chest> Chests;

        public Burglar()
        {
            Chests = new List<Chest>();
        }
    }
}