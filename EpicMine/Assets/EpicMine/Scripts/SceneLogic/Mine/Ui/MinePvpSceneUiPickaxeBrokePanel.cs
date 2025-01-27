using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using TMPro;
using UnityEngine;

public class MinePvpSceneUiPickaxeBrokePanel : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _timer;

    private void Start()
    {
        EventManager.Instance.Subscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeBroken);
        EventManager.Instance.Subscribe<PvpArenaEndGameEvent>(OnGameEnd);
    }

    public void OnGameEnd(PvpArenaEndGameEvent ev)
    {
        StopAllCoroutines();
        _root.SetActive(false);
    }

    public void OnPickaxeBroken(MineScenePickaxeDestroyedEvent ev)
    {
        _root.SetActive(true);
        _timer.text = 0.ToString();
        StartCoroutine(_timerCorutine());
    }

    private IEnumerator _timerCorutine()
    {
        var waitTime = PvpLocalConfig.DefaultPvpMinePickaxeRestoreTIme;

        while (waitTime > 0)
        {
            _timer.text = waitTime.ToString();
            yield return new WaitForSeconds(1);
            waitTime--;
        }

        _root.SetActive(false);

    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeBroken);
            EventManager.Instance.Unsubscribe<PvpArenaEndGameEvent>(OnGameEnd);
        }
    }
}
