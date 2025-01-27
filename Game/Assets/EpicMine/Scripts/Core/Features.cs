using System.Collections.Generic;
using BlackTemple.Common;
using UnityEngine;


namespace BlackTemple.EpicMine.Core
{
    public class Features
    {
        public List<CommonDLL.Static.FeaturesType> FeaturesList;

        public Features(CommonDLL.Dto.Features data)
        {
            FeaturesList = data.FeaturesList;
        }

        public void Add(CommonDLL.Static.FeaturesType featuresType)
        {
            Debug.Log(featuresType);
            if (FeaturesList.Contains(featuresType))
                return;

            FeaturesList.Add(featuresType);

            Debug.Log(featuresType);
            EventManager.Instance.Publish(new AddNewFeatureEvent(featuresType));
        }

        public bool Exist(CommonDLL.Static.FeaturesType feature)
        {
            return FeaturesList.Contains(feature);
        }
    }
}