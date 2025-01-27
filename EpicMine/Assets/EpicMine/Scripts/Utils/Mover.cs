
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

public class Mover : MonoBehaviour
{

    public MovingType MoveType;

    public void Start()
    {
        var startPosition = transform.localPosition;
        switch (MoveType)
        {
            case MovingType.FlyingUpAndDown:
                transform.DOLocalMove(new Vector3(startPosition.x, startPosition.y + 0.3f), 4)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine, 0.5f);
                break;
        }
    }
}
