using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using DG.Tweening;
using UnityEngine;

public class MineSceneHand : MonoBehaviour
{
    [SerializeField] private Vector3 _usePosition;
    [SerializeField] private Vector3 _useRotation;

    private Vector3 _startPosition;
    private Vector3 _startRotation;

    private void Start()
    {
        _startRotation = transform.localEulerAngles;
        _startPosition = transform.localPosition;

        EventManager.Instance.Subscribe<MineSceneTorchUseEvent>(OnUseTorch);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<MineSceneTorchUseEvent>(OnUseTorch);
    }

    private void OnUseTorch(MineSceneTorchUseEvent eventData)
    {
        if(eventData.IsStart)
            UseTorch();
        else UseTorchEnd();
    }


    public void UseTorch()
    {
        transform.DOKill();
        transform.DOLocalRotate(_useRotation, 0.2f);
        transform.DOLocalMove(_usePosition, 0.2f);
    }

    public void UseTorchEnd()
    {
        transform.DOKill();
        transform.DOLocalMove(_startPosition, 0.2f);
        transform.DOLocalRotate(_startRotation, 0.2f);
    }
}
