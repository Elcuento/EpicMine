using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using DragonBones;
using UnityEngine;

public class AutoMinerAnimationController : MonoBehaviour
{
    [SerializeField] private string _idleAnimationName;
    [SerializeField] private List<string> _randomAnimationNames = new List<string>();
    [SerializeField] private UnityArmatureComponent _armature;

    private bool _isWork;

    private void Start()
    {
        EventManager.Instance.Subscribe<AutoMinerStartEvent>(OnAutoMinerStart);
        EventManager.Instance.Subscribe<AutoMinerEndEvent>(OnAutoMineEnd);

        Initialize();
    }

    private void Initialize()
    {
        _isWork = true;

        if (_randomAnimationNames.Count == 0)
        {
            _armature.animation.FadeIn(_idleAnimationName, 0.1f);
        }
        else
        {
            StartCoroutine(AnimationCycle());
        }

        if (!App.Instance.Player.AutoMiner.Started || App.Instance.Player.AutoMiner.IsFull)
        {
            _isWork = false;
            _armature.animation.Play("wait");
        }
        else { _armature.animation.Play("work"); }
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
        _isWork = true;
        _armature.animation.Play("work");
    }

    private void OnAutoMineEnd(AutoMinerEndEvent eventData)
    {
        _isWork = false;
        _armature.animation.Play("wait");
    }

    private IEnumerator AnimationCycle()
    {
        while (true)
        {
            if (!_isWork)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

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
