using UnityEngine;

namespace BlackTemple.EpicMine
{
    [CreateAssetMenu(fileName = "ReferencesTables")]
    public class ReferencesTables : ScriptableObject
    {
        public SpritesReferencesTable Sprites;

        public MusicReferencesTable Music;

        public SoundsReferencesTable Sounds;

        public ColorReferencesTable Colors;

        public FileDataReferencesTable FileData;
    }
}