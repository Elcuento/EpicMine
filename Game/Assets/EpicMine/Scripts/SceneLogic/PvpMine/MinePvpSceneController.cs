using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MinePvpSceneController : MineSceneBaseController
    {
        [SerializeField] private PvpArenaNetworkController _photon;

        private int _sectionCount => PvpLocalConfig.DefaultPvpMineSectionCount;

        private PvpArenaMatchType _matchType;
        private int _timeLeft;

        private Coroutine _timer;
        
        private IEnumerator _timerCounter(long time)
        {
            var timeEnd = TimeManager.Instance.NowUnixSeconds + time;

            while (true)
            {
                var timeNow = TimeManager.Instance.NowUnixSeconds;
                var timeRest = timeEnd - timeNow;
                _timeLeft += 1; 

                if (timeRest > 0)
                {
                    EventManager.Instance.Publish(new PvpArenaTimeTickEvent(timeRest));
                }
                else
                {
                    EventManager.Instance.Publish(new PvpArenaTimeTickEvent(0));
                   
                    yield break;
                }
                yield return new WaitForSeconds(1);
            }
        }

  
        protected override void Subscribe()
        {
            base.Subscribe();

            EventManager.Instance.Subscribe<PvpArenaStartGameEvent>(OnGameStart);
            EventManager.Instance.Subscribe<PvpArenaPlayerLeaveGameEvent>(OnPlayerLeave);
            EventManager.Instance.Subscribe<PvpArenaEndGameEvent>(OnGameEnd);
            EventManager.Instance.Subscribe<PvpArenaOpponentSectionPassedEvent>(OnOpponentSectionPassed);
            EventManager.Instance.Subscribe<PvpArenaOnFindChest>(OnFindChestEvent);
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<PvpArenaPlayerLeaveGameEvent>(OnPlayerLeave);
                EventManager.Instance.Unsubscribe<PvpArenaStartGameEvent>(OnGameStart);
                EventManager.Instance.Unsubscribe<PvpArenaEndGameEvent>(OnGameEnd);
                EventManager.Instance.Unsubscribe<PvpArenaOpponentSectionPassedEvent>(OnOpponentSectionPassed);
                EventManager.Instance.Unsubscribe<PvpArenaOnFindChest>(OnFindChestEvent);
            }
        }

        public void OnSpeedHackDetected()
        {
            App.Instance.Services.AnalyticsService.CustomEvent("speed_hack", 
                new CustomEventParameters
                {
                    String = new Dictionary<string, string>
                    {
                        { "id", App.Instance.Player.Id }
                    }
                });

            Time.timeScale = 0;

            DOTween.Sequence()
                .SetDelay(5)
                .OnComplete(UnHacked)
                .SetUpdate(true);
        }

        public void UnHacked()
        {
            Time.timeScale = 1;
        }


        protected override void Initialize()
        {
            var matchData = PvpArenaNetworkController.GetMatchData();

            if (matchData == null)
            {
                Debug.LogError("match data is null");
                PvpArenaNetworkController.DestroyMatchData();
                SceneManager.Instance.LoadScene(ScenesNames.PvpArena);
                return;
            }
            _matchType = matchData.Type;
            CreateSectionProvider();
            SectionProvider.Sections.FirstOrDefault()?.SetReady();
        }

        protected override void CreateSectionProvider()
        {
            SectionProvider = new PvpSectionProvider(_sectionCount, _hero);
        }

        #region network callbacks

        private void OnPlayerLeave(PvpArenaPlayerLeaveGameEvent eventData)
        {
            if (_timer != null) StopCoroutine(_timer);
        }

        private void OnGameEnd(PvpArenaEndGameEvent data)
        {
            if (_timer != null)
                StopCoroutine(_timer);

            if (_isGameEnd) return;
            _isGameEnd = true;

            var player = App.Instance.Player;

            App.Instance.Services.AnalyticsService.EndPvpMatch((player.Dungeon.LastOpenedTier.Number + 1),
                player.Prestige, player.Pvp.Rating, _timeLeft, _photon.GetOpponentProperty<bool>(PvpArenaUserPropertyType.IsBot),
                data.Resoult, _photon.GetOpponentProperty<int>(PvpArenaUserPropertyType.Wall));

        }

        private void OnGameStart(PvpArenaStartGameEvent data)
        {
            Initialize();

            _hero.Initialize(SectionProvider.Sections.FirstOrDefault());

            var endTime = TimeManager.Instance.NowUnixSeconds + data.Info.MatchTime;
            var timeLeft = endTime - TimeManager.Instance.NowUnixSeconds;

            _timer = StartCoroutine(_timerCounter(timeLeft));
        }

        private void OnFindChestEvent(PvpArenaOnFindChest eventData)
        {
            _photon.SetPlayerProperty(PvpArenaUserPropertyType.Chest, (int)eventData.Type);
        }

        public void OnOpponentSectionPassed(PvpArenaOpponentSectionPassedEvent data)
        {
            if (_sectionCount == 0) return;

            if (data.Number >= _sectionCount)
            {
              //  Debug.Log("SET WINNER OTHER");
               // _photon.SetWinnerOther();
            }
        }
 

#endregion

        protected override void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            if (_isGameEnd)
                return;

            StartCoroutine(MoveHero(eventData.Delay, eventData.Section));

            var getDamage = 0;

            var wallSection = eventData.Section as MineScenePvpWallSection;

            if (wallSection != null)
            {
                getDamage = wallSection.DamageReceived;
            }
           
            _photon.SetPlayerProperty(new Dictionary<PvpArenaUserPropertyType, object>
            {
                { PvpArenaUserPropertyType.Damage, _photon.GetPlayerProperty<int>(PvpArenaUserPropertyType.Damage) + getDamage },
                { PvpArenaUserPropertyType.Wall, eventData.Section.Number + 1 }
            });
        }

        protected override IEnumerator MoveHero(float delay, MineSceneSection passedSection)
        {
           // delay -= 0.5f;

            var timeStart = TimeManager.Instance.NowUnixSeconds;
            yield return new WaitUntil(()=> _photon.GetPlayerProperty<int>(PvpArenaUserPropertyType.Wall) >= passedSection.Number + 1);

            var waitLeft = (int)(TimeManager.Instance.NowUnixSeconds - timeStart);

            delay -= waitLeft;
            delay = delay <= 0 ? 0 : delay;

            yield return new WaitForSeconds(delay);

            _hero.MoveToNextNotEmptySection();
        }

        protected override void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            var passedSections = SectionProvider.Sections.Where(s => (s.IsPassed || s is MineSceneEmptySection) && s.Number < (_hero.CurrentSection != null ? _hero.CurrentSection.Number : 0)).ToList();
            foreach (var passedSection in passedSections)
            {
                SectionProvider.Sections.Remove(passedSection);
                Destroy(passedSection.gameObject);
            }
        }

        protected override void OnPickaxeDestroyed(MineScenePickaxeDestroyedEvent data)
        {
            StartCoroutine(_timerPickaxeRestoreHealth());
        }

        private IEnumerator _timerPickaxeRestoreHealth()
        {
            var waitTime = 5f;
            while (waitTime > 0)
            {
                yield return new WaitForSeconds(1);
                waitTime--;
                Hero.Pickaxe.RefillHealth();
            }
        }

    }
}
