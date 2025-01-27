namespace BlackTemple.EpicMine
{
    public struct EffectEndBuffEvent
    {
        public Core.Buff Buff;

        public EffectEndBuffEvent(Core.Buff buff)
        {
            Buff = buff;
        }
    }
}