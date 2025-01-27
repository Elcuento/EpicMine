using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneHeroBuff : MineSceneBuff
    {
        protected MineSceneHero _hero;

        public virtual void Initialize(MineSceneHero hero)
        {
            _hero = hero;
        }
    }
}