namespace BlackTemple.EpicMine
{
    public struct MineSceneHeroBuffsChangeEvent
    {
        public MineSceneHero Hero;

        public MineSceneHeroBuffsChangeEvent(MineSceneHero hero)
        {
            Hero = hero;
        }
    }
}