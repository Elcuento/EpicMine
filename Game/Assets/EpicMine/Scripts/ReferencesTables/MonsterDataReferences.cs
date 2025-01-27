using UnityEngine;

namespace BlackTemple.EpicMine
{
    [CreateAssetMenu(fileName = "MonsterDataReferences")]
    public class MonsterDataReferences : ScriptableObject
    {
        [Header("Audio")]
        public AudioClip[] Waiting;
        public AudioClip Release;
        public AudioClip Death;
        public AudioClip[] TakeDamage;
        public AudioClip[] AttackEnd;
        public AudioClip AbilityEnd;
        public AudioClip JumpAway;

        [Header("Sizes")]
        public float MaxSize;
        public float MinSize;

        [Header("Positions")]
        public float StayDistance;
        public float AwayDistance;


    }
}