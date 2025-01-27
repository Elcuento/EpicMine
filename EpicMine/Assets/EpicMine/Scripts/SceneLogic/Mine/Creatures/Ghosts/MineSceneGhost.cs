using System;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using DragonBones;
using UnityEngine;
using Random = UnityEngine.Random;

public class MineSceneGhost : MonoBehaviour
{
    private Action<string, Action> _onSpeak;
    private Action _onLeave;

    [SerializeField] private GhostAppearType _appearType;
    [SerializeField] private GhostFlyType _flyType;
    [SerializeField] private UnityArmatureComponent _animator;

    private Ghost _staticGhost;
    private ClickHandler _clickHandler;


    private string[] _dialogue = new string[0];
    private CharacterType _charType;
    private GhostActionType _actionType;
    private bool _isReady;
    private int _flyDirection;
    private bool _isHorizontal;
    private int _stage;
   


    public void Initiate(ClickHandler clickHandler, Action onLeave, Action<string, Action> onSpeak, Ghost ghost, GhostActionType action)
    {
    //    action = Random.Range(0, 2); //0;

        _staticGhost = ghost;
        _charType = _staticGhost.Id;

        if (!string.IsNullOrEmpty(_staticGhost.Dialogues))
        {
            var localizeDialog = LocalizationHelper.GetLocale(_staticGhost.Dialogues);
            _dialogue = localizeDialog.Split('#');
        }
      

        _actionType = action;
        

        _onLeave = onLeave;
        _onSpeak = onSpeak;
        _clickHandler = clickHandler;
        _clickHandler.PointClickEvent += ClickOn;

       
        _stage = 0;
        _isReady = false;
        _animator.gameObject.SetActive(false);

        if (_actionType == GhostActionType.Speak)
        {
            if (_appearType == GhostAppearType.Animation)
            {
                _animator.gameObject.SetActive(false);

                DOTween.Sequence().SetDelay(0.5f)
                    .OnComplete(() =>
                    {
                        _animator.gameObject.SetActive(true);
                        _animator.animation.Play("Release", 1);
                        DOTween.Sequence()
                            .SetDelay(_animator.animation.animations["Release"].duration)
                            .OnComplete(() =>
                            {

                                _animator.animation.FadeIn("Waiting", 0.1f);
                                DoActionSpeak();
                            }).SetUpdate(true);
                    })
                    .SetUpdate(true);
            }
            else if (_appearType == GhostAppearType.FromForwardWithFade)
            {
                
                transform.localPosition = new Vector3(0.3f, 3f, -5);
                transform.localScale = new Vector3(0.1f,0.1f,0.1f);

                DOTween.Sequence().SetDelay(0.5f)
                    .OnComplete(
                        () =>
                        {
                            _animator.gameObject.SetActive(true);
                            _animator.animation.Play("Release", 1);

                            transform.DOScale(new Vector3(1, 1, 1), 1);
                            transform.DOLocalMoveZ(0, 0);
                            
                            DOTween.Sequence()
                                .SetDelay(_animator.animation.animations["Release"].duration)
                                .OnComplete(() =>
                                {

                                    _animator.animation.FadeIn("Waiting", 0.1f);
                                    DoActionSpeak();
                                }).SetUpdate(true);

                        }).SetUpdate(true);
            }
        }
        else
        {
            if (_flyType == GhostFlyType.LeftToRightReverse)
            {
                _isHorizontal = true;
                _flyDirection = Random.Range(0, 100) > 50 ? 1 : -1;

                transform.localPosition += new Vector3(_flyDirection * 8, 0);
                DOTween.Sequence()
                    .SetDelay(0.75f)
                    .OnComplete(() =>
                    {
                        _animator.gameObject.SetActive(true);
                        DoActionFlyAround();
                    })
                    .SetUpdate(true);
            } else if (_flyType == GhostFlyType.DownToUpDouble)
            {
                DOTween.Sequence()
                    .SetDelay(0.5f)
                    .OnComplete(() =>
                    {
                        _animator.gameObject.SetActive(true);

                        var isFly = Random.Range(0, 100) > 50;

                        if (isFly)
                        {
                            _flyDirection = Random.Range(0, 100) > 50 ? 1 : -1;
                            _isHorizontal = true;
                            transform.localPosition += new Vector3(_flyDirection * 8, 0);
                            DoActionFlyAround();
                        }
                        else
                        {
                            _isHorizontal = false;

                            _animator.animation.Play("Fly_up", 1);
                            DOTween.Sequence()
                                .SetDelay(1.5f)
                                .OnComplete(() => { DoActionLeave(true); })
                                .SetUpdate(true);
                        }
                    }).SetUpdate(true);
            }
            
        }
            
    }

    public void DoActionSpeak()
    {
        _isReady = false;
        if (_dialogue.Length > _stage)
        {
            _onSpeak(_dialogue[_stage], SetRead);
            _animator.animation.FadeIn("Tap", 0.1f, 1);

            DOTween.Sequence()
                .SetDelay(_animator.animation.animations["Tap"].duration)
                .OnComplete(() =>
                {
                    _animator.animation.FadeIn("Waiting", 0.1f);
                })
                .SetUpdate(true);

       
            _stage++;

        }
        else
        {

            _animator.animation.FadeIn("Tap", 0.1f, 1);
            DOTween.Sequence()
                .SetDelay(_animator.animation.animations["Tap"].duration)
                .OnComplete(() =>
                {
                    _animator.animation.FadeIn("Waiting", 0.1f);
                    DoActionLeave();
                })
                .SetUpdate(true);
            
        }

    }

    public void DoActionFlyAround()
    {
        _isReady = false;

        if (_isHorizontal)
        {
            _animator.animation.FadeIn("Fly", 0.1f, 1);

            DOTween.Sequence().SetDelay(2).OnComplete(() => { DoActionLeave(true); }).SetUpdate(true);

            transform.DOLocalMoveX(-_flyDirection * 8, 2)
                .SetEase(Ease.Linear).SetUpdate(true);

            transform.localEulerAngles = new Vector3(0, -_flyDirection < 0 ? 0 : 180, 0);
        }
        else
        {
            _animator.animation.FadeIn("Fly", 0.1f, 1);

            DOTween.Sequence().SetDelay(2).OnComplete(() => { DoActionLeave(true); }).SetUpdate(true);

            transform.DOLocalMoveY(10, 3)
                .SetEase(Ease.Linear).SetUpdate(true);
        }
    }

    public void DoActionLeave(bool immediately = false)
    {
        var task = App.Instance.Player.Quests.IsExistInSpeakQuest(_charType);
        task?.Speak(_charType);

        _isReady = false;
        var scale = transform.localScale;

        Recolor(0.75f, Color.black);

        transform.DOScale(new Vector3(scale.x * 0.5f, scale.y * 0.5f, scale.z * 0.5f), immediately ? 0 : 0.75f)
            .SetEase(Ease.Linear)
            .OnComplete(() => { Destroy(gameObject); }).SetUpdate(true);

        _onLeave?.Invoke();
    }

    public void Recolor(float speed, Color color)
    {
        var allMats = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var mat in allMats)
        {
            mat.material.DOColor(color, speed);
        }
    }

    public void Fade(float to, float speed, Color color)
    {
        var allMats = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var mat in allMats)
        {
            mat.material.DOColor(new Color(color.r, color.g, color.b, to), speed).SetUpdate(true);
        }
    }

    public void OnDestroy()
    {
        if(_clickHandler!=null)
        _clickHandler.PointClickEvent -= ClickOn;
    }

    public void SetRead()
    {
        _isReady = true;
    }
    public void ClickOn()
    {
        if (_isReady)
        {
            if(_actionType == GhostActionType.Speak)
             DoActionSpeak();
        }
    }
}
