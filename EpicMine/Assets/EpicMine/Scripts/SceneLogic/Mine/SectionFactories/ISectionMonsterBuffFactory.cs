

using CommonDLL.Static;

namespace BlackTemple.EpicMine
{
    public interface ISectionMonsterBuffFactory
    {
        MineSceneMonsterSelfBuff CreateSelfBuff(MonsterAbility id);

        MineSceneMonsterOtherBuff CreateOtherBuff(MonsterAbility id, MineSceneHero hero);
    }
}