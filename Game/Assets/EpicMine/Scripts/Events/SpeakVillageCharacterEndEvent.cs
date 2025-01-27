

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct SpeakVillageCharacterEndEvent
    {
        public CharacterType Type;

        public SpeakVillageCharacterEndEvent(CharacterType type)
        {
            Type = type;
        }
    }
}