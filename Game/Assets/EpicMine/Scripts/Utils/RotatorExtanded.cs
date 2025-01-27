
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;
// ReSharper disable IdentifierTypo

public class RotatorExtanded : MonoBehaviour {

    public RotateType RotateType;

    public void Start()
    {
        var startPosition = transform.localPosition;

        switch (RotateType)
        {
            case RotateType.RotateAround:
                transform.DORotate(new Vector3(0, 0, 360), 20, RotateMode.LocalAxisAdd)
                    .SetLoops(-1)
                    .SetEase(Ease.Linear);
                break;
        }
    }
}
