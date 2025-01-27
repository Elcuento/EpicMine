using System.Collections.Generic;


namespace CommonDLL.Dto
{
    public class Blacksmith
    {
        public string SelectedPickaxe;

        public List<Pickaxe> Pickaxes;

        public Dictionary<string,int> AdPickaxes;

        public Blacksmith()
        {
            Pickaxes = new List<Pickaxe>();
            AdPickaxes = new Dictionary<string, int>();
        }
    }
}