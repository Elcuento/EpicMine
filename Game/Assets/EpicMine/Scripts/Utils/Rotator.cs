using UnityEngine;

namespace BlackTemple.EpicMine.Utils
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _axis;
        [SerializeField] private float _speed;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }

        private void Update()
        {
            _transform.Rotate(_axis, _speed * Time.unscaledDeltaTime);
        }
    }
}