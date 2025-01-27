using System;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackPointMonster : MineSceneAttackPoint
    {
        public MineSceneAttackPointAttach AttachPoint { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        [SerializeField] private SpriteRenderer _healthBar;

        [SerializeField] private Sprite[] _monsterPoints;
        [SerializeField] private Sprite _defaultPoint;

        public void Initialize(int size, AttackPointType type, MineSceneAttackPointAttach attach, float delay = 0)
        {
            //  SingletonPool.Instance.Clear(); // TODO

            Clear();

            AttachPoint = attach;
            AttachPoint.SetBusy();

            _inner.gameObject.SetActive(true);
            _outer.gameObject.SetActive(true);

            Size = size;
            PointType = type;

            switch (PointType)
            {
                case AttackPointType.MonsterReflectPoint:
                    _inner.enabled = false;
                    var index = (Size / 2) - 1;
                    _outer.sprite = _monsterPoints[index >= 0 ? index : 0];
                    SetColor("#800080");
                    MaxHealth = size <= 2 ? 1 : size <= 3 ? 3 : 5;
                    Health = MaxHealth;
                    SetHealth();
                    break;

                case AttackPointType.MonsterWeakPoint:
                    _inner.enabled = false;
                    var index2 = (Size / 2) - 1;
                    _outer.sprite = _monsterPoints[index2 >= 0 ? index2 : 0];
                    SetColor("#05F3F5");
                    MaxHealth = size <= 2 ? 1 : size <= 3 ? 3 : 5;
                    Health = MaxHealth;
                    SetHealth();
                    break;

                case AttackPointType.Health:
                    _outer.sprite = _defaultPoint;
                    _inner.enabled = true;
                    SetColor(Color.red);
                    SetSize();
                    MaxHealth = 0;
                    Health = MaxHealth;
                    SetHealth();
                    break;
            }

            PlaySpawnAnimation(delay);
        }

        protected override void SetColor(string colorStr)
        {
            base.SetColor(colorStr);
            _healthBar.color = Extensions.GetHexColor(colorStr);
        }

        private void SetSize()
        {
            var size = Size * MineLocalConfigs.PointPartSize;
            transform.localScale = new Vector3(size, size);
        }

        public override bool Hit(AttackPointHitType hitType, bool isHorizontalHit = true)
        {

            switch (PointType)
            {
                case AttackPointType.Empty:
                case AttackPointType.Default:
                case AttackPointType.Energy:
                case AttackPointType.Health:
                case AttackPointType.Random:
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

                        if (Health > 1)
                        {
                            Health--;
                            SetHealth();
                            return false;
                        }

                        _inner.gameObject.SetActive(false);
                        _outer.gameObject.SetActive(false);

                        _inner.DOFade(0f, 1f).SetEase(Ease.InQuad);
                        _outer.DOFade(0f, 1f).SetEase(Ease.InQuad)
                            .OnComplete(DestroyPoint);

                        _destroyInner.localEulerAngles = new Vector3(0f, 0f, isHorizontalHit ? -90f : 0);

                        return true;
                    }

                    _destroyOuter.Play();

                    if (Health > 1)
                    {
                        Health--;
                        SetHealth();
                        return false;
                    }

                    _inner.DOFade(0.25f, 0);
                    _outer.DOFade(0.25f, 0);

                    _inner.DOFade(0f, 0.5f)
                        .SetEase(Ease.InQuad);

                    _outer.DOFade(0f, 0.5f)
                        .SetEase(Ease.InQuad)
                        .OnComplete(DestroyPoint);

                    return true;
                case AttackPointType.MonsterWeakPoint:
                case AttackPointType.MonsterReflectPoint:

                    _destroyOuter.Play();
                    var scale = transform.localScale.x;
                    transform.DOScale(new Vector3(scale -0.2f, scale-0.2f, scale-0.2f), 0.2f).SetLoops(2, LoopType.Yoyo);

                    if (Health > 1)
                    {
                        Health--;
                        SetHealth();
                        return false;
                    }

                    _outer.DOFade(0.25f, 0);
                    _outer.DOFade(0f, 0.5f)
                        .SetEase(Ease.InQuad)
                        .OnComplete(DestroyPoint);

                    return true;
            }

            return false;
        }

        private void SetHealth()
        {
            if (MaxHealth == 0)
            {
                _healthBar.transform.localScale = new Vector3(0,0.3f,1);
                return;
            }

            var scale = Size == 3 ? 5 : Size == 4 ? 8.3f : 12;
            var newValue = (Health / (float)MaxHealth) * scale;

            _healthBar.transform.localPosition = new Vector3(-OuterBounds.x, -OuterBounds.y - 0.4f);
   
            _healthBar.transform.localScale = new Vector3(newValue, 0.3f, 1);
        }

        private void Update()
        {
            if (AttachPoint == null)
                return;

            transform.position = AttachPoint.transform.position;
            Size = Size;
        }

        protected override void DestroyPoint()
        {
            base.DestroyPoint();
            AttachPoint.SetFree();
        }
    }
}
