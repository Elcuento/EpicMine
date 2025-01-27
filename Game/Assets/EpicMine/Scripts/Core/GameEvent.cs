using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class GameEvent
    {
        public CommonDLL.Static.GameEvent StaticGameEvent;

        public bool IsActive;

        public long EndTime;

        public long Left => EndTime - TimeManager.Instance.NowUnixSeconds;

        public GameEvent(CommonDLL.Static.GameEvent gameEvent, CommonDLL.Dto.GameEvent gameEventDto)
        {
            StaticGameEvent = gameEvent;
            
            IsActive =  gameEventDto.IsActive;
            EndTime = gameEventDto.EndTime;
        }

        public void Initialize()
        {
            if (IsActive)
            {
                foreach (var quest in StaticGameEvent.Quests)
                {
                    var exist = App.Instance.Player.Quests.QuestList.Find(x => x.StaticQuest.Id == quest);

                    if (exist != null)
                        continue;

                    var staticQuest = App.Instance.StaticData.Quests.Find(x => x.Id == quest);

                    if (staticQuest == null)
                    {
                        App.Instance.Services.LogService.Log("Quest not exist " + quest + " on event " + StaticGameEvent.Id);
                        continue;
                    }

                    App.Instance.Player.Quests.Add(staticQuest);
                }
            }
            else
            {
                foreach (var quest in StaticGameEvent.Quests)
                {
                    var exist = App.Instance.Player.Quests.QuestList.Find(x => x.StaticQuest.Id == quest);

                    App.Instance.Player.Quests.Remove(exist);
                }
            }

        }
    }
}