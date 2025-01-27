

namespace BlackTemple.EpicMine
{
    public struct AddNewFeatureEvent
    {
        public CommonDLL.Static.FeaturesType Type;

        public AddNewFeatureEvent(CommonDLL.Static.FeaturesType type)
        {
            Type = type;
        }
    }
}