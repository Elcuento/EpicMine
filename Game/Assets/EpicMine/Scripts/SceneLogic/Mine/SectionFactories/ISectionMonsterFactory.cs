

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public interface ISectionMonsterFactory
    {
        MineSceneMonster CreateMonster(MonsterType type, string id);
    }
}