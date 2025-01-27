using System;
using System.Collections.Generic;
using BlackTemple.EpicMine;

namespace BlackTemple.Common
{
    public class EventManager : Singleton<EventManager>
    {
        public int EventsCount => _events.Count;

        readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>(32);

        public void Subscribe<T>(EventHandler<T> eventAction)
        {
            if (eventAction != null)
            {
                var eventType = typeof(T);
                Delegate rawList;
                _events.TryGetValue(eventType, out rawList);
                _events[eventType] = (rawList as EventHandler<T>) + eventAction;
            }
        }

        public void Unsubscribe<T>(EventHandler<T> eventAction, bool keepEvent = false)
        {
            if (eventAction != null)
            {
                var eventType = typeof(T);
                Delegate rawList;
                if (_events.TryGetValue(eventType, out rawList))
                {
                    var list = (rawList as EventHandler<T>) - eventAction;
                    if (list == null && !keepEvent)
                    {
                        _events.Remove(eventType);
                    }
                    else
                    {
                        _events[eventType] = list;
                    }
                }
            }
        }

        public void UnsubscribeAll<T>()
        {
            var eventType = typeof(T);
            Delegate rawList;
            if (_events.TryGetValue(eventType, out rawList))
                _events.Remove(eventType);
        }

        public void UnsubscribeAndClearAllEvents()
        {
            _events.Clear();
        }

        public void Publish<T>(T eventMessage)
        {
            var eventType = typeof(T);
            Delegate rawList;
            _events.TryGetValue(eventType, out rawList);
            var list = rawList as EventHandler<T>;
            if (list == null)
                return;

            try
            {
                list(eventMessage);
            }
            catch (Exception ex)
            {
                App.Instance.Services.LogService.LogError($"EventManager: {ex.Message}");
            }
        }
    }
}