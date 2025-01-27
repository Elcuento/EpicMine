using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackPoint : MonoBehaviour
    {
        public AttackPointType PointType { get; protected set; }
        public int Size { protected set; get; }

        public Vector3 InnerBounds => _inner.bounds.extents;
        public Vector3 OuterBounds => _outer.bounds.extents;

        [SerializeField] protected SpriteRenderer _inner;
        [SerializeField] protected SpriteRenderer _outer;

        [SerializeField] protected ParticleSystem _destroyOuter;
        [SerializeField] protected Transform _destroyInner;
        [SerializeField] protected Animator _animator;


     
        public virtual bool Hit(AttackPointHitType hitType, bool isHorizontalHit = true)
        {

            if (hitType == AttackPointHitType.Inner)
            {
                var animationName = AttackPointsLocalConfig.GreenDestroyInnerAnimationName;
                switch (PointType)
                {
                    case AttackPointType.Energy:
                        animationName = AttackPointsLocalConfig.YellowDestroyInnerAnimationName;
                        break;
                    case AttackPointType.Health:
                        animationName = AttackPointsLocalConfig.RedDestroyInnerAnimationName;
                        break;
                }

                _animator.Play(animationName);
                _inner.gameObject.SetActive(false);
                _outer.gameObject.SetActive(false);

                _inner.DOFade(0f, 1f).SetEase(Ease.InQuad);
                _outer.DOFade(0f, 1f).SetEase(Ease.InQuad)
                    .OnComplete(DestroyPoint);

                _destroyInner.localEulerAngles = new Vector3(0f, 0f, isHorizontalHit ? -90f : 0);

                return true;
            }

            _destroyOuter.Play();

            _inner.DOFade(0.25f, 0);
            _outer.DOFade(0.25f, 0);

            _inner.DOFade(0f, 0.5f)
                .SetEase(Ease.InQuad);

            _outer.DOFade(0f, 0.5f)
                .SetEase(Ease.InQuad)
                .OnComplete(DestroyPoint);

            return true;
        }

        protected virtual void DestroyPoint()
        {
            SingletonPool.Instance.ToPool(gameObject);
        }
        
        protected virtual void PlaySpawnAnimation(float delay = 0)
        {
            _inner.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            _outer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            _inner.DOFade(1, AttackPointsLocalConfig.InnerFirstStepDuration)
                .SetDelay(delay)
                .SetEase(Ease.InQuad);

            _inner.transform
                .DOScale(1.1f, AttackPointsLocalConfig.InnerFirstStepDuration)
                .SetDelay(delay)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _inner.transform
                        .DOScale(1f, AttackPointsLocalConfig.InnerSecondStepDuration)
                        .SetEase(Ease.OutQuad);
                });

            _outer.DOFade(1, AttackPointsLocalConfig.OuterSecondStepDuration)
                .SetDelay(delay + AttackPointsLocalConfig.OuterFirstStepDuration)
                .SetEase(Ease.InQuad);

            _outer.transform
                .DOScale(1.2f, AttackPointsLocalConfig.OuterSecondStepDuration)
                .SetDelay(delay + AttackPointsLocalConfig.OuterFirstStepDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _outer.transform
                        .DOScale(1f, AttackPointsLocalConfig.OuterThirdStepDuration)
                        .SetEase(Ease.Linear);
                });
        }

        protected virtual void Clear()
        {
            _inner.color = new Color(1, 1, 1, 0);
            _outer.color = new Color(1, 1, 1, 0);

            _inner.gameObject.SetActive(false);
            _outer.gameObject.SetActive(true);

            _animator.Rebind();
        }

        protected virtual void SetColor(string colorStr)
        {
            var color = Extensions.GetHexColor(colorStr);

            _inner.color = color;
            _outer.color = color;

            var psMain = _destroyOuter.main;
            var col = new Color(color.r, color.g, color.b, 1);
            psMain.startColor = col;
        }
        protected virtual void SetColor(Color color)
        {
            _inner.color = color;
            _outer.color = color;

            var psMain = _destroyOuter.main;
            var col = new Color(color.r, color.g, color.b, 1);
            psMain.startColor = col;
        }
    }
}