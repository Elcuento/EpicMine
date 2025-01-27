using System;
using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using UnityEngine;
using Random = UnityEngine.Random;

public class PvpArenaStatiscticHandler : MonoBehaviour
{
    private List<int> _sectionPassTime;

    private long _startTime;


    public void Awake()
    {
        _sectionPassTime = new List<int>();
    }

    public void Start()
    {
        if (Random.Range(0, 100) < PvpLocalConfig.SendSectionPassStatisticChance)
        {
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Subscribe<PvpArenaEndGameResoultEvent>(OnGameEnd);
        }
        else
        {
            enabled = false;
        }
    }

    public void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Unsubscribe<PvpArenaEndGameResoultEvent>(OnGameEnd);
        }
    }

    public void OnSectionPassed(MineSceneSectionPassedEvent eventData)
    {
        if (eventData.Section.Number == 0)
        {
            _startTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }
        if (eventData.Section.Number > 0)
        {
            var passTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() - _startTime;
            _sectionPassTime.Add((int)passTime);
            _startTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }
    }

    public void OnGameEnd(PvpArenaEndGameResoultEvent eventData)
    {
        if (_sectionPassTime.Count == 0)
            return;


        var arena = PvpArenaNetworkController.GetMatchData().Arena;

        var avarenge = 0;

        foreach (var i in _sectionPassTime)
             avarenge += i;

        avarenge = avarenge / _sectionPassTime.Count;
       // App.Instance.Player.Pvp.SendStatistic(arena, avarenge);
    }
       
}
