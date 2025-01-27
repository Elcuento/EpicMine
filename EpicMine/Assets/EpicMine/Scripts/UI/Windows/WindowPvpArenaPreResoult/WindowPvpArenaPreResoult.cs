using System;
using System.Collections;
using System.Collections.Generic;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using DragonBones;
using TMPro;
using UnityEngine;

public class WindowPvpArenaPreResoult : WindowBase {

    [SerializeField] private UnityArmatureComponent _header;
    [SerializeField] private GameObject _rotatedSpark;
    [SerializeField] private TextMeshProUGUI _label;


    public void Initialize(PvpArenaGameResoultType resoult, Action onEnd)
    {
        _rotatedSpark.transform.DORotate(new Vector3(0, 0, 359), 3, RotateMode.FastBeyond360)
            .SetLoops(-1);

        var str = resoult == PvpArenaGameResoultType.Win ? "win" :
            resoult == PvpArenaGameResoultType.Loose ? "loose" : "none";

        _label.text = LocalizationHelper.GetLocale($"pvp_game_resoult_{resoult}");
        StartCoroutine(ShowHeaderResoult(str, onEnd));
    }

    public IEnumerator ShowHeaderResoult(string headerAnim, Action onEnd)
    {
        var anim = _header.animation.Play(headerAnim,1);

        yield return new WaitForSeconds(anim._duration + 1);
        WindowManager.Instance.Close(this, true);

        onEnd?.Invoke();
    }
}
