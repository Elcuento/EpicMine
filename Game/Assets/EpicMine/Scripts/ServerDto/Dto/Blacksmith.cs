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
        public Blacksmith(BlackTemple.EpicMine.Core.Blacksmith data)
        {
            SelectedPickaxe = data.SelectedPickaxe.StaticPickaxe.Id;

            Pickaxes = new List<Pickaxe>();
            AdPickaxes = new Dictionary<string, int>();

            foreach (var dataPickax in data.Pickaxes)
            {
                Pickaxes.Add(new Pickaxe(dataPickax.StaticPickaxe.Id, dataPickax.IsCreated, dataPickax.IsHiltFound));
            }
            foreach (var dataPickax in data.AdPickaxes)
            {
                AdPickaxes.Add(dataPickax.Key, dataPickax.Value);
            }
        }
    }
}