using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class VillageSceneParallax : MonoBehaviour
    {
        [SerializeField] private ScrollRect _world;
        [SerializeField] private ScrollRect _godChest;
        [SerializeField] private ScrollRect _mountainsFront;
        [SerializeField] private ScrollRect _mountainsBack;

        private Vector3 _lastPosition;

        public void OnScroll(Vector2 value)
        {
            if (_lastPosition == _world.content.transform.localPosition)
                return;

            _lastPosition = _world.content.transform.localPosition;

            _godChest.horizontalNormalizedPosition = value.x;
            _mountainsFront.horizontalNormalizedPosition = value.x;
            _mountainsBack.horizontalNormalizedPosition = value.x;
        }
    }
}