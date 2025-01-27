using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class Tnt
    {
        public string Id { get; }

        public float DamagePercent;

        public Tnt(float damagePercent, string id)
        {
            DamagePercent = damagePercent * 0.01f;
            Id = id;
        }
    }
}