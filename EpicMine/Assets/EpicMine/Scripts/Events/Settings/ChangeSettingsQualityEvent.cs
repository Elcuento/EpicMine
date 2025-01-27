namespace BlackTemple.EpicMine
{
    public struct ChangeSettingsQualityEvent
    {
        public bool IsLow;

        public ChangeSettingsQualityEvent(bool isLow)
        {
            IsLow = isLow;
        }
    }
}