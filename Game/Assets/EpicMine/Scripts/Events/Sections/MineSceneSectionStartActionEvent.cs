

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public struct MineSceneSectionStartActionEvent
    {
        public MineSceneMonsterSection Section;
        public MonsterActionType MonsterAction;
        public float Time;

        public MineSceneSectionStartActionEvent(MineSceneMonsterSection section, MonsterActionType type, float time)
        {
            MonsterAction = type;
            Section = section;
            Time = time;
        }
    }
}