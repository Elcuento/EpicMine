using System.Collections.Generic;
using CommonDLL.Static;


namespace CommonDLL.Dto
{
    public class Features
    {
        public List<FeaturesType> FeaturesList;

        public Features()
        {
            FeaturesList = new List<FeaturesType>();
        }
        public Features(BlackTemple.EpicMine.Core.Features data)
        {
            FeaturesList = new List<FeaturesType>();

            foreach (var featuresType in data.FeaturesList)
            {
                FeaturesList.Add(featuresType);
            }
        }
    }
}