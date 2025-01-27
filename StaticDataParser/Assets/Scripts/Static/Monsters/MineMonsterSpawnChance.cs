using System.Collections.Generic;
using BlackTemple.Common;
using Newtonsoft.Json;

namespace BlackTemple.EpicMine.Static
{
    public class MineMonsterSpawnChance
    {
        public int MaxCount;
        public Dictionary<string, int> Chance;

        public MineMonsterSpawnChance()
        {
            Chance = new Dictionary<string, int>();
        }

        [JsonConstructor]
        public MineMonsterSpawnChance(string maxCountChance)
        {
            Chance = new Dictionary<string, int>();

            if (string.IsNullOrEmpty(maxCountChance))
                return;

            var spawnDic = Extensions.GetDictionaryBySplitKeyValuePair<string, int>(maxCountChance, '#');

            foreach (var i in spawnDic)
            {
                if (i.Key == "max")
                {
                    MaxCount = i.Value;
                    break;
                }
            }

            spawnDic.Remove("max");

            Chance = spawnDic;
        }
    }
}