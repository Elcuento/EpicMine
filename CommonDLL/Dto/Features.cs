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
    }
}