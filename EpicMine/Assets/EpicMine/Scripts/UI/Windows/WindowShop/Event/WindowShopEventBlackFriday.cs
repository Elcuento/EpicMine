using System;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

public class WindowShopEventBlackFriday : MonoBehaviour
{
    [SerializeField] private GameObject _blackFridayContent;
    [SerializeField] private GameObject _usualTitleContent;

    [SerializeField] private TextMeshProUGUI _eventDayTimer;

    private BlackTemple.EpicMine.Core.GameEvent _event;


    private void Start()
    {
        if (App.Instance.GameEvents.IsActive(GameEventType.BlackFriday))
        {
            _event = App.Instance.GameEvents.GetEvent(GameEventType.BlackFriday);

            _blackFridayContent.SetActive(true);
            _usualTitleContent.SetActive(false);

            if (_event.StaticGameEvent.ExpireType == GameEventExpireType.Time)
            {
                _eventDayTimer.text = TimeHelper.SecondsToDate(_event.EndTime);

                EventManager.Instance.Subscribe<UnscaledSecondsTickEvent>(OnTickTime);

                OnTickTime(new UnscaledSecondsTickEvent());
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
        if (TimeManager.Instance.NowUnixSeconds > _event.EndTime)
        {
            End();
            return;
        }

        var date = new DateTime();
         date = date.AddSeconds(_event.Left);

        _eventDayTimer.text = TimeHelper.Format(date, true);
    }

    private void End()
    {

        _blackFridayContent.SetActive(false);
        _usualTitleContent.SetActive(true);

        UnSubscribe();
    }

}
