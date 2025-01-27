using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.EpicMine.Dto
{
    public struct TutorialUnRowBase
    {
        public TutorialUnRowStepIds Id;

        public TutorialUnRowBase(TutorialUnRowStepIds id)
        {
            Id = id;
        }

        public TutorialUnRowBase(Core.TutorialUnRowBase data)
        {
            Id = data.Id;
        }
    }
}