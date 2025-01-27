using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using DragonBones;
using UnityEngine;
// ReSharper disable IdentifierTypo

public class AutoMinerCustomAnimationController : MonoBehaviour
{
    [SerializeField] private string _idleAnimationName;
    [SerializeField] private string _startStopAnimationName;
    [SerializeField] private string _endStopAnimationName;
    [SerializeField] private string _stopAnimationName;

    [SerializeField] private List<string> _randomAnimationNames = new List<string>();
    [SerializeField] private UnityArmatureComponent _armature;


    private bool _isStopped;
    // ReSharper disable once IdentifierTypo
    private Coroutine _moveCoroutine;


    private void Start()
    {
        EventManager.Instance.Subscribe<AutoMinerStartEvent>(OnAutoMinerStart);
        EventManager.Instance.Subscribe<AutoMinerEndEvent>(OnAutoMineEnd);

        if (!App.Instance.Player.AutoMiner.Started || App.Instance.Player.AutoMiner.IsFull)
        {
            _armature.animation.Play(_stopAnimationName);
            _isStopped = true;
            // Stop();

        }
        else Initialize();
    }


    private void Stop()
    {
        if(_moveCoroutine!=null)
           StopCoroutine(_moveCoroutine);

        StartCoroutine(StopCoroutine());
    }

    private void Initialize()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(StartCoroutine());
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<AutoMinerStartEvent>(OnAutoMinerStart);
        EventManager.Instance.Unsubscribe<AutoMinerEndEvent>(OnAutoMineEnd);
    }


    private void OnAutoMinerStart(AutoMinerStartEvent eventData)
    {
        Initialize();
    }

    private void OnAutoMineEnd(AutoMinerEndEvent eventData)
    {
        Stop();
    }

    private IEnumerator StopCoroutine()
    {

        yield return new WaitForSecondsRealtime(_armature.animation.animations[_startStopAnimationName].duration);

        _armature.animation.FadeIn(_startStopAnimationName, 0.1f, 1);

        yield return new WaitForSecondsRealtime(_armature.animation.animations[_stopAnimationName].duration);
        _armature.animation.FadeIn(_stopAnimationName, 0.1f);
        _isStopped = true;

    }

    private IEnumerator StartCoroutine()
    {
        if (_isStopped)
        {
            _armature.animation.FadeIn(_endStopAnimationName, 0.1f);
            yield return new WaitForSecondsRealtime(_armature.animation.animations[_endStopAnimationName].duration);
        }
        while (true)
        {
            _armature.animation.FadeIn(_idleAnimationName, 0.1f);

            var randomTime = Random.Range(5f, 10f);
            yield return new WaitForSecondsRealtime(randomTime);

            var randomAnimationIndex = Random.Range(0, _randomAnimationNames.Count);
            var randomAnimationName = _randomAnimationNames[randomAnimationIndex];

            _armature.animation.FadeIn(randomAnimationName, 0.1f, 1);
            yield return new WaitForSecondsRealtime(_armature.animation.animations[randomAnimationName].duration);
        }
    }
}
