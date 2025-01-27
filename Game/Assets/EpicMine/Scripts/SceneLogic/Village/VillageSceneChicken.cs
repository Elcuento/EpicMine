using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using DG.Tweening;
using DragonBones;
using UnityEngine;
using Random = UnityEngine.Random;

public class VillageSceneChicken : MonoBehaviour
{
    public Action OnClick;
    public Action<Vector2> OnFall;

    [SerializeField] private Camera _camera;

    [SerializeField] private List<RectTransform> _spawnArea;
    [SerializeField] private UnityArmatureComponent _armature;
    [SerializeField] private Rigidbody2D _rigid;

    [SerializeField] private float _moveSpeedMin = 22;
    [SerializeField] private float _moveSpeedMax = 50;

    private RectTransform _currentArea;
    private float _maxRightPosition;
    private float _maxLeftPosition;

    private bool _isDrag;
    private bool _isGround;

    private void Start()
    {
        if (App.Instance.Player.AutoMiner.Level <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        SetArea(GetRandomSpawnArea());
        RandomAction();
    }

    private void SetArea(RectTransform area, bool randomizePosition = true)
    {
        _currentArea = area;

        _maxRightPosition = _currentArea.transform.localPosition.x + _currentArea.sizeDelta.x;
        _maxLeftPosition = _currentArea.transform.localPosition.x;

        if (randomizePosition)
        {
            transform.localPosition = new Vector3(Random.Range(_maxLeftPosition, _maxRightPosition),
                _currentArea.transform.localPosition.y, 1);
        }

    }

    private RectTransform GetRandomSpawnArea()
    {
        return _spawnArea.RandomElement();
    }

    private void SetClosesSpawnArea()
    {
        var area = _spawnArea.OrderBy(x => Math.Abs(x.transform.position.y - transform.position.y))
            .FirstOrDefault();

        SetArea(area,false);
    }

    private void MoveTo(float positionX)
    {
        if (positionX < transform.localPosition.x && transform.localEulerAngles.y != 180)
        {
            StartCoroutine(Rotate(new Vector2(0, 180), () => MoveTo(positionX)));
            return;
        }

        if (positionX > transform.localPosition.x && transform.localEulerAngles.y != 0)
        {
            StartCoroutine(Rotate(new Vector2(0, 0), () => MoveTo(positionX)));
            return;
        }

        var speed = Random.Range(_moveSpeedMin, _moveSpeedMax);
        var coefficient = speed / _moveSpeedMax;
        var animationSpeed = 1 + coefficient;

        _armature.animation.FadeIn("01_walk", 0.1f);
        _armature.animation.timeScale = animationSpeed;

        var distance = Mathf.Abs(Mathf.Abs(positionX) - Mathf.Abs(transform.localPosition.x));

        transform.DOLocalMoveX(positionX, distance / speed)
            .SetEase(Ease.Linear)
            .OnComplete(MoveEnd);
    }

    private void MoveEnd()
    {
        RandomAction();
    }


    private void RandomMove()
    {
        MoveTo(Random.Range(_maxLeftPosition, _maxRightPosition));
    }

    private void RandomAction()
    {

        var randomVal = Random.Range(0, 100);

        if (randomVal <= 10)
        {
            StartCoroutine(Sleep());

        }
        else if (randomVal <= 30)
        {
            StartCoroutine(Peck());
        }
        else if (randomVal <= 50)
        {
            StartCoroutine(Etc());
        }
        else if (randomVal <= 70)
        {
            StartCoroutine(Jump());
        }
        else if (randomVal <= 100)
        {
            RandomMove();
        }
    }

    private IEnumerator Rotate(Vector3 vector, Action onEnd)
    {
        _armature.animation.FadeIn("03_twist", 0.1f, 1);
        yield return new WaitForSecondsRealtime(_armature.animation.animations["03_twist"].duration);
        transform.localEulerAngles = vector;
        transform.localPosition = vector.y >= 180
            ? new Vector3(transform.localPosition.x + 20, transform.localPosition.y, transform.localPosition.y)
            : new Vector3(transform.localPosition.x - 20, transform.localPosition.y, transform.localPosition.y);
        _armature.animation.Play("01_walk", 1);
        onEnd?.Invoke();
    }

    private IEnumerator Peck()
    {
        var randomVal = Random.Range(1, 5);
        while (randomVal > 0)
        {
            var randomVal2 = Random.Range(0, 100);
            var animName = randomVal2 < 30 ? "06_peck_jump" : randomVal2 < 60 ? "04_peck_one" : "05_peck_three";
            _armature.animation.FadeIn(animName, 0.1f, 1);
            yield return new WaitForSecondsRealtime(_armature.animation.animations[animName].duration);
            randomVal--;
        }

        RandomAction();
    }

    private IEnumerator Sleep()
    {
        _armature.animation.FadeIn("09_sleep_start", 0.1f, 1);
        yield return new WaitForSecondsRealtime(_armature.animation.animations["09_sleep_start"].duration);
        _armature.animation.FadeIn("10_sleep", 0.1f);
        yield return new WaitForSecondsRealtime(_armature.animation.animations["10_sleep"].duration +
                                                Random.Range(0, 10));
        _armature.animation.FadeIn("11_sleep_end", 0.1f, 1);
        yield return new WaitForSecondsRealtime(_armature.animation.animations["11_sleep_end"].duration);
        RandomAction();
    }

    private IEnumerator Jump()
    {
        var randomVal = Random.Range(1, 5);
        while (randomVal > 0)
        {
            _armature.animation.FadeIn("02_jump", 0.1f, 1);
            yield return new WaitForSecondsRealtime(_armature.animation.animations["02_jump"].duration);
            randomVal--;
        }

        RandomAction();
    }

    private IEnumerator Etc()
    {
        var animName = Random.Range(0, 100) < 50 ? "07_headshake" : "08_dig";
        _armature.animation.FadeIn(animName, 0.1f, 1);
        yield return new WaitForSecondsRealtime(_armature.animation.animations[animName].duration);
        RandomAction();
    }

    private IEnumerator Tap()
    {
        _armature.animation.FadeIn("12_tap", 0.1f, 1);
        yield return new WaitForSecondsRealtime(_armature.animation.animations["12_tap"].duration);
        RandomAction();
    }

    public void OnPress()
    {
        _isDrag = true;

        _armature.animation.FadeIn("12_tap", 0.1f);
        StopAllCoroutines();
        transform.DOKill();

        OnClick?.Invoke();
    }

    private void Update()
    {
        if (_isDrag)
        {
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            //var newPos = new Vector3(pos.x, pos.y, transform.position.z);

          //  GetComponent<Rigidbody2D>().position = new Vector3(pos.x, pos.y, transform.position.z);
           transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }

        if (transform.position.y <= -100)
        {
            ReSpawn();
        }
    }

    public void ReSpawn()
    {
        StopAllCoroutines();
        transform.DOKill();
        SetArea(GetRandomSpawnArea());
        RandomAction();
    }


    public void OnRelease()
    {
        _isDrag = false;

        if (_isGround)
        {
            StopAllCoroutines();
            transform.DOKill();
            SetClosesSpawnArea();
            RandomAction();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _isGround = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isGround)
            return;

        if (!_isDrag)
        {
            _isGround = true;
            StopAllCoroutines();
            transform.DOKill();
            SetClosesSpawnArea();
            RandomAction();
        }
        else
        {
            _isGround = true;
            OnFall?.Invoke(_rigid.velocity);
        }
    }
}
