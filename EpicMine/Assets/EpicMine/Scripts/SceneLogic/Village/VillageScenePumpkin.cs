using System.Collections;
using BlackTemple.EpicMine;
using DG.Tweening;
using UnityEngine;

public class VillageScenePumpkin : VillageSceneCharacter
{
    [SerializeField] private CanvasGroup _leftEye;
    [SerializeField] private CanvasGroup _rightEye;

    protected override void Start()
    {
        _leftEye.alpha = 0.1f;
        _rightEye.alpha = 0.1f;

        _leftEye.DOFade(0.5f, 0.75f)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutBounce);

        _rightEye.DOFade(0.5f, 0.75f)
            .SetLoops(-1, LoopType.Yoyo);

        base.Start();


    }
}
