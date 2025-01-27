using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using DG.Tweening;
using UnityEngine;

public class MineSceneAttackLine : MonoBehaviour
{
    private Action _onChangeDirection;

    [SerializeField] private MineSceneAttackLineHit _hitPrefab;

    public bool IsHorizontal;
    public float MoveTime { get; set; }

    private float _direction;
    private Tweener _moveTween;

    private bool _rotating;


    public void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<MineSectionAttackLineRotationChangeEvent>(OnDirectionChanged);
    }

    public void Initialize(bool isHorizontal, float speed, Action onChangeDirection)
    {
      
        IsHorizontal = isHorizontal;
        MoveTime = speed;
        _onChangeDirection = onChangeDirection;

        var attackLinePosition = Vector3.zero;
        _direction = 1;

        if (isHorizontal) attackLinePosition.x = MineLocalConfigs.HorizontalAttackLineMaxXPosition;
        else attackLinePosition.y = MineLocalConfigs.VerticalAttackLineMaxYPosition;

        transform.localEulerAngles = new Vector3(0, 0, isHorizontal ? 90 : 0);
        transform.localPosition = -attackLinePosition;
        transform.localScale = new Vector3(isHorizontal ? 0.47f : 0.55f, transform.localScale.y, transform.localScale.z);

        if (isHorizontal) MoveHorizontal();
        else MoveVertical();

        EventManager.Instance.Subscribe<MineSectionAttackLineRotationChangeEvent>(OnDirectionChanged);
    }

    #region Moving

    private void MoveHorizontal(bool back = false, float speedC = 1)
    {
        _direction = back ? _direction * -1 : _direction;
        _moveTween.Kill();

        _moveTween = transform
            .DOLocalMoveX(
                MineLocalConfigs.HorizontalAttackLineMaxXPosition * _direction,
                speedC * MoveTime)
            .SetEase(speedC == 1 ? Ease.InOutSine : Ease.OutSine)
            .OnComplete(() => { MoveHorizontal(true); });

        _onChangeDirection?.Invoke();
    }

    private void MoveVertical(bool back = false, float speedC = 1)
    {
        _direction = back ? _direction * -1 : _direction;
        _moveTween.Kill();


        _moveTween = transform
            .DOLocalMoveY(
                MineLocalConfigs.VerticalAttackLineMaxYPosition * _direction,
                speedC * MoveTime)
            .SetEase(speedC == 1 ? Ease.InOutSine : Ease.OutSine)
            .OnComplete(() => { MoveVertical(true); });

        _onChangeDirection?.Invoke();
    }

    #endregion


    public void Hit()
    {
        if (_rotating)
            return;

        var hit = Instantiate(_hitPrefab, transform.parent, false);
        hit.Initialize(transform);

        
    }
    #region Events

    public void Rotate()
    {

        return;
        if (IsHorizontal)
        {
            transform.DOScale(new Vector3(0f, 0.53f), 0.2f)
                .OnComplete(() =>
            {
                var xCoef = Math.Abs(transform.localPosition.x) / MineLocalConfigs.HorizontalAttackLineMaxXPosition;

                _direction = -_direction;

                var newPosition = new Vector3(0, MineLocalConfigs.VerticalAttackLineMaxYPosition * xCoef * _direction);
                transform.localEulerAngles = new Vector3(0, 0, 0);
                transform.localPosition = newPosition;
                IsHorizontal = !IsHorizontal;

                MoveVertical(speedC: 1 - xCoef);

                transform.DOScale(new Vector3(0.47f, 0.53f), 0.2f)
                    .OnComplete(() => { _rotating = false; });
            });

        }
        else
        {
            transform.DOScale(new Vector3(0, 0.53f), 0.2f)
                .OnComplete(() =>
            {
                var yCoef = Math.Abs(transform.localPosition.y) / MineLocalConfigs.VerticalAttackLineMaxYPosition;

                _direction = -_direction;

                var newPosition = new Vector3(MineLocalConfigs.HorizontalAttackLineMaxXPosition * yCoef * _direction, 0);
                transform.localEulerAngles = new Vector3(0, 0, 90);
                transform.localPosition = newPosition;
                IsHorizontal = !IsHorizontal;

                MoveHorizontal(speedC: 1 - yCoef);

                transform.DOScale(new Vector3(0.47f, 0.53f), 0.2f)
                    .OnComplete(() => { _rotating = false; });

            });
        }
    }



    public void OnDirectionChanged(MineSectionAttackLineRotationChangeEvent data)
    {
        return;
        if (_rotating)
            return;

        _rotating = true;

        Rotate();
    }

    #endregion
}
