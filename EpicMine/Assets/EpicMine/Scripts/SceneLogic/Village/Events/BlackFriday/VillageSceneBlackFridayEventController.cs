using System;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

public class VillageSceneBlackFridayEventController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _blackFridayContent;

    [SerializeField] private TextMeshProUGUI _eventDayTimer;

    private BlackTemple.EpicMine.Core.GameEvent _event;

    private void Start()
    {

        if (App.Instance.GameEvents.IsActive(GameEventType.BlackFriday))
        {
            _event = App.Instance.GameEvents.GetEvent(GameEventType.BlackFriday);

            foreach (var o in _blackFridayContent)
            {
                o.SetActive(true);
            }

            if (_event.StaticGameEvent.ExpireType == GameEventExpireType.Time)
            {
                _eventDayTimer.text = TimeHelper.SecondsToDate(_event.EndTime);

                EventManager.Instance.Subscribe<UnscaledSecondsTickEvent>(OnTickTime);
            }
        }
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    private void UnSubscribe()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<UnscaledSecondsTickEvent>(OnTickTime);
    }

    private void OnTickTime(UnscaledSecondsTickEvent eventData)
    {
        var left = _event.Left;
        var date = DateTime.UtcNow.AddMilliseconds(left);

        if (TimeManager.Instance.NowUnixSeconds > _event.EndTime)
        {
            End();
            return;
        }

        _eventDayTimer.text = 
           string.Format(LocalizationHelper.GetLocale("black_friday_village_timer_title"), TimeHelper.GetDaysBetweenDates(date));
    }

    private void End()
    {
        foreach (var o in _blackFridayContent)
        {
            o.SetActive(false);
        }

        UnSubscribe();
    }



    public void OnClickBanner()
    {
        WindowManager.Instance.Show<WindowShop>(withCurrencies: true)
            .OpenCrystals();
    }
}
