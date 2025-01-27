using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackLineHit : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void Initialize(Transform target)
        {
            transform.position = target.position;
            transform.localEulerAngles = target.localEulerAngles;
            transform.localScale = target.localScale;

            _spriteRenderer
                .DOFade(0.8f, 0)
                .SetUpdate(true)
                .OnComplete(() => {
                    _spriteRenderer
                        .DOFade(0, MineLocalConfigs.AttackHitFadeTime)
                        .SetUpdate(true)
                        .OnComplete(() => { Destroy(gameObject); });
                });
        }
    }
}