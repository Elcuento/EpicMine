
namespace BlackTemple.EpicMine.Static
{
    public class Prestige
    {
        public float CriticalPercent { get; }
        public float FortunePercent { get; }
        public float GoldPercent { get; }
        public float WallHealthPercent { get; }

        public Prestige(float criticalPercent, float fortunePercent, float goldPercent, float wallHealthPercent)
        {
            CriticalPercent = criticalPercent;
            FortunePercent = fortunePercent;
            GoldPercent = goldPercent;
            WallHealthPercent = wallHealthPercent;
        }
    }
}
