using BlackTemple.Common;
using CommonDLL.Dto;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowFlyingIcons : WindowBase
    {
        [SerializeField] private float _duration;
        [SerializeField] private float _scale;
        [SerializeField] private float _offset;

        [Space]
        [SerializeField] private RectTransform _container;
        [SerializeField] private ItemView _flyingItemPrefab;

        private Pool<ItemView> _pool;


        public void Create(Item item, Vector3 viewportPosition, string targetTag, float delay = 0)
        {
            var target = GameObject.FindGameObjectWithTag(targetTag);
            if (target == null)
                return;

            var newItem = _pool.Instantiate();
            newItem.Initialize(item);

            Move(newItem, viewportPosition, target.transform, delay);
        }

        public void Create(Dto.Currency currency, Vector3 viewportPosition, string targetTag, float delay = 0)
        {
            var target = GameObject.FindGameObjectWithTag(targetTag);
            if (target == null)
                return;

            var newItem = _pool.Instantiate();
            newItem.Initialize(currency);

            Move(newItem, viewportPosition, target.transform, delay);
        }

        public void Create(Sprite icon, string text, Vector3 viewportPosition, string targetTag, float delay = 0)
        {
            var target = GameObject.FindGameObjectWithTag(targetTag);
            if (target == null)
                return;

            var newItem = _pool.Instantiate();
            newItem.Initialize(icon, text);

            Move(newItem, viewportPosition, target.transform, delay);
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
            _pool = new Pool<ItemView>(_flyingItemPrefab, _container, 0);
        }


        private void Move(ItemView item, Vector3 viewportPosition, Transform target, float delay = 0)
        {
            var rect = item.GetComponent<RectTransform>();
            var targetPosition = Canvas.worldCamera.WorldToViewportPoint(target.position);

            viewportPosition.x *= _container.sizeDelta.x;
            viewportPosition.y *= _container.sizeDelta.y;

            targetPosition.x *= _container.sizeDelta.x;
            targetPosition.y *= _container.sizeDelta.y;

            rect.localScale = Vector3.zero;
            rect.anchoredPosition = viewportPosition;

            rect.DOScale(Vector3.one, _scale).OnComplete(() =>
            {
                var randomOffset = Random.Range(-_offset, _offset);
                rect.DOJumpAnchorPos(targetPosition, randomOffset, 1, _duration)
                    .OnComplete(() =>
                    {
                        _pool.Destroy(item);
                    });
            }).SetDelay(delay);
        }
    }
}