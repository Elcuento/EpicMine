using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using TMPro;
using UnityEngine;

public class WindowCrystalSubscription : WindowBase
{
    private Buff _buff;

    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private TextMeshProUGUI _daysLeft;


    public void Start()
    {
        EventManager.Instance.Subscribe<SecondsTickEvent>(OnTick);
    }

    public void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTick);
    }

    public void Initialize(Buff buff)
    {        
        _buff = buff;

        OnTick(new SecondsTickEvent());
    }

    public void OnTick(SecondsTickEvent data)
    {
        if (_buff.TimeLeft < 0)
        {
           OnEnd();
        }
        else
        {
            var checkTime = new DateTime();
            var totalTime = new DateTime();
            checkTime = checkTime.AddMilliseconds(_buff.TimeLeftToCheck);
            totalTime = totalTime.AddMilliseconds(_buff.TimeLeft);

            _timer.text = checkTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            _daysLeft.text = $"{LocalizationHelper.GetLocale("window_crystal_subscription_days_left")} {totalTime.ToString("dd", CultureInfo.InvariantCulture)}";
        }
    }

    public void OnEnd()
    {
        Close();
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTick);
    }
}
