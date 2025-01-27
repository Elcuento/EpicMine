using UnityEngine;

namespace BlackTemple.EpicMine
{
    [CreateAssetMenu(fileName = "Music")]
    public class MusicReferencesTable : ScriptableObject
    {
        public AudioClip Mine;
        public AudioClip MineBoss;
        public AudioClip Village;
    }
}