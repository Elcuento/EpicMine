using System.Collections.Generic;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class GameEvents
    {
        public List<GameEvent> Events;

        public GameEvents(List<CommonDLL.Dto.GameEvent> events)
        {
            Events = new List<GameEvent>();

            if (events != null)
            {
                foreach (var gameEvent in events)
                {
                    var staticEvent = App.Instance.StaticData.GameEvents.Find(x => x.Id == gameEvent.Id);
                    if (staticEvent != null)
                    {
                        App.Instance.Services.LogService.Log($"Event {gameEvent.Id} status {gameEvent.IsActive}");
                        Events.Add(new GameEvent(staticEvent, gameEvent));
                    }else App.Instance.Services.LogService.Log($"Event {gameEvent.Id} not exist");
                }
            }
        }

        public void Initialize()
        {
            foreach (var gameEvent in Events)
            {
                gameEvent.Initialize();
            }
        }

        public bool IsActive(GameEventType type)
        {
            var currentEvent = Events.Find(x => x.StaticGameEvent.Type == type);

            return currentEvent != null && currentEvent.IsActive;
        }

        public GameEvent GetEvent(GameEventType type)
        {
            return Events.Find(x => x.StaticGameEvent.Type == type);
        }
    }
}