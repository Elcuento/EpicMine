using System.Collections;
using System.Collections.Generic;
using BlackTemple.EpicMine;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WindowPvpArenaPreStart : WindowBase
{
    [SerializeField] private TextMeshProUGUI _timer;

    private IEnumerator _timerCorutine(float time)
    {
        while (time > 0)
        {
            _timer.text = time.ToString();
            _timer.transform.DOPunchScale(new Vector3(1.3f, 1.3f), 0.1f);
            yield return new WaitForSeconds(1);
            time -= 1;
        }

        _timer.text = "GO";
        _timer.transform.DOPunchScale(new Vector3(1.3f, 1.3f), 0.1f);
        yield return new WaitForSeconds(1);

        Close();
    }

    public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
    {
        base.OnShow(withPause, withCurrencies);
        StartCoroutine(_timerCorutine(PvpLocalConfig.DefaultPvpMineStartTime));
    }


}
