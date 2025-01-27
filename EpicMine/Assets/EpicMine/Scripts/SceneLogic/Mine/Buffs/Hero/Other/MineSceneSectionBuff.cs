
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneSectionBuff : MonoBehaviour
    {
        public AbilityType Type { get; protected set; }
        protected MineSceneSection _section;

        public virtual void Initialize(MineSceneSection section, AbilityType type)
        {
            _section = section;
            Type = type;
        }

        public virtual void Clear() { }
    }
}