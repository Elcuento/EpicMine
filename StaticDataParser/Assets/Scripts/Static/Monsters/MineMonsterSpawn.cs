using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class MineMonsterSpawn
    {
        public List<MineMonsterSpawnChance> SpawnChances;

        public MineMonsterSpawn(string mine1, string mine2, string mine3, string mine4, string mine5)
        {
            SpawnChances = new List<MineMonsterSpawnChance>
            {
                new MineMonsterSpawnChance(mine1),
                new MineMonsterSpawnChance(mine2),
                new MineMonsterSpawnChance(mine3),
                new MineMonsterSpawnChance(mine4),
                new MineMonsterSpawnChance(mine5)
            };               
        }
    }
}