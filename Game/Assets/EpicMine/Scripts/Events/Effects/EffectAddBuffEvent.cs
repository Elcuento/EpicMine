namespace BlackTemple.EpicMine
{
    public struct EffectAddBuffEvent
    {
        public Core.Buff Buff;

        public EffectAddBuffEvent(Core.Buff buff)
        {
            Buff = buff;
        }
    }
}