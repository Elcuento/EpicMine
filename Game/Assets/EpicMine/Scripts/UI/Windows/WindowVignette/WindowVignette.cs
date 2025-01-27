using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowVignette : WindowBase
    {
        [SerializeField] private Image[] _images;

        public void Initialize(Color color, float time = 1f)
        {
            Clear();

            foreach (var image in _images)
            {
                image.color = color;
                image.DOFade(1, 0f).SetUpdate(true);
                image.DOFade(0, time).SetUpdate(true);
            }
        }

        private void Clear()
        {
            foreach (var image in _images)
            {
                image.DOKill();
                image.DOFade(0, 0).SetUpdate(true);
            }
        }
    }
}