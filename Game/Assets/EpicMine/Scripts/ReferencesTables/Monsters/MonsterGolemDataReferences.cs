using UnityEngine;
// ReSharper disable IdentifierTypo

namespace BlackTemple.EpicMine
{
    [CreateAssetMenu(fileName = "MonsterDataReferences")]
    public class MonsterGolemDataReferences : MonsterDataReferences
    {
        [Header("Audio")]
        public AudioClip AbilityStart;
        public AudioClip AbilityProgress;
    }
}