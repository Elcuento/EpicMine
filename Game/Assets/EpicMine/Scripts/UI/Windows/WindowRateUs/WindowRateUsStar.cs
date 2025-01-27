using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowRateUsStar : MonoBehaviour
    {
        [SerializeField] private GameObject _full;

        public void Initialize(bool isFull)
        {
            _full.SetActive(isFull);
        }
    }
}