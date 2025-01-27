

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct SpeakVillageCharacterStartEvent
    {
        public CharacterType Type;

        public SpeakVillageCharacterStartEvent(CharacterType type)
        {
            Type = type;
        }
    }
}