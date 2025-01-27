using UnityEngine;

namespace BlackTemple.EpicMine
{
    [CreateAssetMenu(fileName = "MonsterDataReferences")]
    public class MonsterDemonDataReferences : MonsterDataReferences
    {
        [Header("Audio")]
        public AudioClip AbilityStart;
        public AudioClip AbilityProgress;

        public AudioClip Ability2Start;
        public AudioClip Ability2Progress;
        public AudioClip Ability2End;
    }
}